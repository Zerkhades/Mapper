using Mapper.Application.Features.Analytics;
using Mapper.Application.Features.Notifications.DTOs;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Mapper.Application.Features.Notifications.Queries;

public class GetSmartNotificationsQueryHandler
    : IRequestHandler<GetSmartNotificationsQuery, List<SmartNotificationDto>>
{
    private readonly IMapperDbContext _db;

    public GetSmartNotificationsQueryHandler(IMapperDbContext db)
    {
        _db = db;
    }

    public async Task<List<SmartNotificationDto>> Handle(
        GetSmartNotificationsQuery request,
        CancellationToken ct)
    {
        var generatedAt = request.To ?? DateTimeOffset.UtcNow;
        var periodStart = request.From ?? generatedAt.AddHours(-24);
        if (periodStart >= generatedAt)
        {
            throw new ArgumentException("Notification period start must be earlier than period end.");
        }

        var top = Math.Clamp(request.Top, 1, 200);
        var offlineGrace = TimeSpan.FromMinutes(Math.Clamp(request.OfflineGraceMinutes, 1, 1440));
        var periodDuration = generatedAt - periodStart;
        var baselineStart = periodStart - periodDuration;

        var cameras = await _db.GeoMarks
            .AsNoTracking()
            .OfType<CameraMark>()
            .Select(x => new CameraLookupItem(x.Id, x.GeoMapId, x.Title))
            .ToListAsync(ct);

        var cameraIds = cameras.Select(x => x.Id).ToHashSet();

        var periodAlerts = await _db.CameraMotionAlerts
            .AsNoTracking()
            .Where(x => x.DetectedAt >= periodStart && x.DetectedAt < generatedAt)
            .Select(x => new AlertLookupItem(
                x.Id,
                x.CameraMarkId,
                x.DetectedAt,
                x.Severity,
                x.MotionPercentage,
                x.IsResolved))
            .ToListAsync(ct);

        var baselineAlerts = await _db.CameraMotionAlerts
            .AsNoTracking()
            .Where(x => x.DetectedAt >= baselineStart && x.DetectedAt < periodStart)
            .Select(x => new AlertLookupItem(
                x.Id,
                x.CameraMarkId,
                x.DetectedAt,
                x.Severity,
                x.MotionPercentage,
                x.IsResolved))
            .ToListAsync(ct);

        var latestStatuses = await GetLatestStatuses(cameraIds, ct);
        var notifications = new List<SmartNotificationDto>();

        notifications.AddRange(BuildOfflineNotifications(cameras, latestStatuses, generatedAt, offlineGrace));
        notifications.AddRange(BuildHighMotionNotifications(cameras, periodAlerts, generatedAt));
        notifications.AddRange(BuildMotionSpikeNotifications(cameras, periodAlerts, baselineAlerts, periodStart, generatedAt));

        return notifications
            .OrderByDescending(x => SeverityRank(x.Severity))
            .ThenByDescending(x => x.OccurredAt)
            .Take(top)
            .ToList();
    }

    private async Task<Dictionary<Guid, StatusLookupItem>> GetLatestStatuses(
        HashSet<Guid> cameraIds,
        CancellationToken ct)
    {
        var statuses = await _db.CameraStatusHistories
            .AsNoTracking()
            .Where(x => cameraIds.Contains(x.CameraMarkId))
            .Select(x => new StatusLookupItem(
                x.CameraMarkId,
                x.IsOnline,
                x.Reason.ToString(),
                x.ChangedAt,
                x.ResponseTimeMs))
            .ToListAsync(ct);

        return statuses
            .GroupBy(x => x.CameraMarkId)
            .Select(group => group.OrderByDescending(x => x.ChangedAt).First())
            .ToDictionary(x => x.CameraMarkId);
    }

    private static IEnumerable<SmartNotificationDto> BuildOfflineNotifications(
        IReadOnlyCollection<CameraLookupItem> cameras,
        IReadOnlyDictionary<Guid, StatusLookupItem> latestStatuses,
        DateTimeOffset generatedAt,
        TimeSpan offlineGrace)
    {
        foreach (var camera in cameras)
        {
            if (!latestStatuses.TryGetValue(camera.Id, out var status) || status.IsOnline)
            {
                continue;
            }

            var offlineFor = generatedAt - status.ChangedAt;
            if (offlineFor < offlineGrace)
            {
                continue;
            }

            yield return new SmartNotificationDto
            {
                Id = $"camera-offline:{camera.Id}:{status.ChangedAt:O}",
                Type = "CameraOffline",
                Severity = offlineFor >= TimeSpan.FromMinutes(30) ? "Critical" : "Warning",
                Title = "Camera is offline",
                Message = $"{camera.Title} has been offline for {Math.Round(offlineFor.TotalMinutes)} minutes.",
                GeoMapId = camera.GeoMapId,
                CameraMarkId = camera.Id,
                CameraTitle = camera.Title,
                OccurredAt = status.ChangedAt,
                GeneratedAt = generatedAt,
                Context =
                {
                    ["reason"] = status.Reason,
                    ["offlineMinutes"] = Math.Round(offlineFor.TotalMinutes).ToString("F0")
                }
            };
        }
    }

    private static IEnumerable<SmartNotificationDto> BuildHighMotionNotifications(
        IReadOnlyCollection<CameraLookupItem> cameras,
        IReadOnlyCollection<AlertLookupItem> periodAlerts,
        DateTimeOffset generatedAt)
    {
        foreach (var alert in periodAlerts.Where(x => x.Severity == MotionSeverity.High && !x.IsResolved))
        {
            var camera = cameras.FirstOrDefault(x => x.Id == alert.CameraMarkId);
            yield return new SmartNotificationDto
            {
                Id = $"high-motion:{alert.Id}",
                Type = "HighMotion",
                Severity = "Critical",
                Title = "High motion alert",
                Message = $"{camera?.Title ?? "Unknown camera"} detected high motion activity.",
                GeoMapId = camera?.GeoMapId,
                CameraMarkId = alert.CameraMarkId,
                CameraTitle = camera?.Title,
                RelatedEntityId = alert.Id,
                OccurredAt = alert.DetectedAt,
                GeneratedAt = generatedAt,
                Context =
                {
                    ["motionPercentage"] = alert.MotionPercentage.ToString("F2", CultureInfo.InvariantCulture),
                    ["motionSeverity"] = alert.Severity.ToString()
                }
            };
        }
    }

    private static IEnumerable<SmartNotificationDto> BuildMotionSpikeNotifications(
        IReadOnlyCollection<CameraLookupItem> cameras,
        IReadOnlyCollection<AlertLookupItem> periodAlerts,
        IReadOnlyCollection<AlertLookupItem> baselineAlerts,
        DateTimeOffset periodStart,
        DateTimeOffset generatedAt)
    {
        foreach (var camera in cameras)
        {
            var currentCount = periodAlerts.Count(x => x.CameraMarkId == camera.Id);
            var baselineCount = baselineAlerts.Count(x => x.CameraMarkId == camera.Id);
            var anomaly = MotionAnomalyDetector.Analyze(new MotionAnomalyInput(
                camera.Id,
                camera.Title,
                currentCount,
                new[] { baselineCount }));

            if (!anomaly.IsAnomaly)
            {
                continue;
            }

            yield return new SmartNotificationDto
            {
                Id = $"motion-spike:{camera.Id}:{periodStart:O}:{generatedAt:O}",
                Type = "MotionSpike",
                Severity = "Warning",
                Title = "Motion activity spike",
                Message = anomaly.Reason,
                GeoMapId = camera.GeoMapId,
                CameraMarkId = camera.Id,
                CameraTitle = camera.Title,
                OccurredAt = generatedAt,
                GeneratedAt = generatedAt,
                Context =
                {
                    ["currentAlertCount"] = anomaly.CurrentAlertCount.ToString(),
                    ["baselineAverageAlertCount"] = anomaly.BaselineAverageAlertCount.ToString("F2", CultureInfo.InvariantCulture),
                    ["zScore"] = anomaly.ZScore.ToString("F2", CultureInfo.InvariantCulture),
                    ["activityRatio"] = anomaly.ActivityRatio.ToString("F2", CultureInfo.InvariantCulture)
                }
            };
        }
    }

    private static int SeverityRank(string severity)
    {
        return severity switch
        {
            "Critical" => 3,
            "Warning" => 2,
            "Info" => 1,
            _ => 0
        };
    }

    private sealed record CameraLookupItem(Guid Id, Guid GeoMapId, string Title);

    private sealed record AlertLookupItem(
        Guid Id,
        Guid CameraMarkId,
        DateTimeOffset DetectedAt,
        MotionSeverity Severity,
        double MotionPercentage,
        bool IsResolved);

    private sealed record StatusLookupItem(
        Guid CameraMarkId,
        bool IsOnline,
        string Reason,
        DateTimeOffset ChangedAt,
        int? ResponseTimeMs);
}

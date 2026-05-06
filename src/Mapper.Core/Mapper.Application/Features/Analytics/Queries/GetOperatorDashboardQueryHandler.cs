using Mapper.Application.Features.Analytics.DTOs;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.Analytics.Queries;

public class GetOperatorDashboardQueryHandler
    : IRequestHandler<GetOperatorDashboardQuery, OperatorDashboardDto>
{
    private readonly IMapperDbContext _db;

    public GetOperatorDashboardQueryHandler(IMapperDbContext db)
    {
        _db = db;
    }

    public async Task<OperatorDashboardDto> Handle(GetOperatorDashboardQuery request, CancellationToken ct)
    {
        var periodEnd = request.To ?? DateTimeOffset.UtcNow;
        var periodStart = request.From ?? periodEnd.AddHours(-24);
        if (periodStart >= periodEnd)
        {
            throw new ArgumentException("Dashboard period start must be earlier than period end.");
        }

        var top = Math.Clamp(request.Top, 1, 20);
        var periodDuration = periodEnd - periodStart;
        var baselineStart = periodStart - periodDuration;

        var cameras = await _db.GeoMarks
            .AsNoTracking()
            .OfType<CameraMark>()
            .Select(x => new CameraLookupItem(x.Id, x.Title))
            .ToListAsync(ct);

        var cameraIds = cameras.Select(x => x.Id).ToHashSet();

        var periodAlerts = await _db.CameraMotionAlerts
            .AsNoTracking()
            .Where(x => x.DetectedAt >= periodStart && x.DetectedAt < periodEnd)
            .Select(x => new AlertLookupItem(
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
                x.CameraMarkId,
                x.DetectedAt,
                x.Severity,
                x.MotionPercentage,
                x.IsResolved))
            .ToListAsync(ct);

        var archivedVideos = await _db.CameraVideoArchives
            .AsNoTracking()
            .CountAsync(x => x.RecordedAt >= periodStart && x.RecordedAt < periodEnd, ct);

        var latestStatuses = await GetLatestStatuses(cameraIds, ct);
        var topActiveCameras = BuildTopActiveCameras(cameras, periodAlerts, top);
        var anomalies = BuildAnomalies(cameras, periodAlerts, baselineAlerts, periodStart, periodEnd, top);
        var problemCameras = BuildProblemCameras(cameras, latestStatuses, top);

        return new OperatorDashboardDto
        {
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            TotalCameras = cameras.Count,
            OnlineCameras = latestStatuses.Count(x => x.Value.IsOnline),
            OfflineCameras = latestStatuses.Count(x => !x.Value.IsOnline),
            UnknownStatusCameras = cameras.Count(x => !latestStatuses.ContainsKey(x.Id)),
            MotionAlerts = periodAlerts.Count,
            UnresolvedMotionAlerts = periodAlerts.Count(x => !x.IsResolved),
            HighSeverityUnresolvedAlerts = periodAlerts.Count(x => x.Severity == MotionSeverity.High && !x.IsResolved),
            ArchivedVideos = archivedVideos,
            AverageMotionPercentage = periodAlerts.Count == 0 ? 0 : Math.Round(periodAlerts.Average(x => x.MotionPercentage), 2),
            TopActiveCameras = topActiveCameras,
            Anomalies = anomalies,
            ProblemCameras = problemCameras
        };
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

    private static List<CameraActivityDto> BuildTopActiveCameras(
        IReadOnlyCollection<CameraLookupItem> cameras,
        IReadOnlyCollection<AlertLookupItem> alerts,
        int top)
    {
        return alerts
            .GroupBy(x => x.CameraMarkId)
            .Select(group =>
            {
                var camera = cameras.FirstOrDefault(x => x.Id == group.Key);
                return new CameraActivityDto
                {
                    CameraMarkId = group.Key,
                    CameraTitle = camera?.Title ?? "Unknown camera",
                    MotionAlerts = group.Count(),
                    UnresolvedAlerts = group.Count(x => !x.IsResolved),
                    AverageMotionPercentage = Math.Round(group.Average(x => x.MotionPercentage), 2)
                };
            })
            .OrderByDescending(x => x.MotionAlerts)
            .ThenByDescending(x => x.AverageMotionPercentage)
            .Take(top)
            .ToList();
    }

    private static List<CameraAnomalyDto> BuildAnomalies(
        IReadOnlyCollection<CameraLookupItem> cameras,
        IReadOnlyCollection<AlertLookupItem> periodAlerts,
        IReadOnlyCollection<AlertLookupItem> baselineAlerts,
        DateTimeOffset periodStart,
        DateTimeOffset periodEnd,
        int top)
    {
        return cameras
            .Select(camera =>
            {
                var currentCount = periodAlerts.Count(x => x.CameraMarkId == camera.Id);
                var baselineBuckets = BuildBaselineBuckets(
                    baselineAlerts.Where(x => x.CameraMarkId == camera.Id),
                    periodStart,
                    periodEnd);

                return MotionAnomalyDetector.Analyze(new MotionAnomalyInput(
                    camera.Id,
                    camera.Title,
                    currentCount,
                    baselineBuckets));
            })
            .Where(x => x.IsAnomaly)
            .OrderByDescending(x => x.ZScore)
            .ThenByDescending(x => x.ActivityRatio)
            .Take(top)
            .Select(x => new CameraAnomalyDto
            {
                CameraMarkId = x.CameraMarkId,
                CameraTitle = x.CameraTitle,
                CurrentAlertCount = x.CurrentAlertCount,
                BaselineAverageAlertCount = x.BaselineAverageAlertCount,
                ZScore = x.ZScore,
                ActivityRatio = x.ActivityRatio,
                Reason = x.Reason
            })
            .ToList();
    }

    private static List<int> BuildBaselineBuckets(
        IEnumerable<AlertLookupItem> baselineAlerts,
        DateTimeOffset periodStart,
        DateTimeOffset periodEnd)
    {
        var baselineStart = periodStart - (periodEnd - periodStart);
        var buckets = new List<int> { 0 };

        foreach (var alert in baselineAlerts)
        {
            if (alert.DetectedAt >= baselineStart && alert.DetectedAt < periodStart)
            {
                buckets[0]++;
            }
        }

        return buckets;
    }

    private static List<CameraStatusSummaryDto> BuildProblemCameras(
        IReadOnlyCollection<CameraLookupItem> cameras,
        IReadOnlyDictionary<Guid, StatusLookupItem> latestStatuses,
        int top)
    {
        return cameras
            .Select(camera =>
            {
                latestStatuses.TryGetValue(camera.Id, out var status);
                return new CameraStatusSummaryDto
                {
                    CameraMarkId = camera.Id,
                    CameraTitle = camera.Title,
                    IsOnline = status?.IsOnline,
                    Status = status is null ? "Unknown" : status.IsOnline ? "Online" : "Offline",
                    ChangedAt = status?.ChangedAt,
                    Reason = status?.Reason,
                    ResponseTimeMs = status?.ResponseTimeMs
                };
            })
            .Where(x => x.IsOnline != true)
            .OrderBy(x => x.IsOnline.HasValue)
            .ThenBy(x => x.ChangedAt)
            .Take(top)
            .ToList();
    }

    private sealed record CameraLookupItem(Guid Id, string Title);

    private sealed record AlertLookupItem(
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

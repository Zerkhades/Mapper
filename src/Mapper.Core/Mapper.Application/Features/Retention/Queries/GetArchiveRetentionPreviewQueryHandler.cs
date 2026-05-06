using Mapper.Application.Features.Retention.DTOs;
using Mapper.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.Retention.Queries;

public class GetArchiveRetentionPreviewQueryHandler
    : IRequestHandler<GetArchiveRetentionPreviewQuery, ArchiveRetentionPreviewDto>
{
    private readonly IMapperDbContext _db;

    public GetArchiveRetentionPreviewQueryHandler(IMapperDbContext db)
    {
        _db = db;
    }

    public async Task<ArchiveRetentionPreviewDto> Handle(
        GetArchiveRetentionPreviewQuery request,
        CancellationToken ct)
    {
        var now = request.Now ?? DateTimeOffset.UtcNow;
        var motionRetentionDays = Math.Max(1, request.MotionVideoRetentionDays);
        var noMotionRetentionDays = Math.Max(1, request.NoMotionVideoRetentionDays);
        var archivedRetentionDays = Math.Max(1, request.ArchivedVideoRetentionDays);
        var take = Math.Clamp(request.Take, 1, 500);

        var query = _db.CameraVideoArchives.AsNoTracking();
        if (request.CameraMarkId.HasValue)
        {
            query = query.Where(x => x.CameraMarkId == request.CameraMarkId.Value);
        }

        var videos = await query
            .Select(x => new
            {
                x.Id,
                x.CameraMarkId,
                x.VideoPath,
                x.ThumbnailPath,
                x.RecordedAt,
                x.FileSizeBytes,
                x.HasMotionDetected,
                x.IsArchived
            })
            .ToListAsync(ct);

        var candidates = videos
            .Select(video => BuildCandidate(
                video.Id,
                video.CameraMarkId,
                video.VideoPath,
                video.ThumbnailPath,
                video.RecordedAt,
                video.FileSizeBytes,
                video.HasMotionDetected,
                video.IsArchived,
                now,
                motionRetentionDays,
                noMotionRetentionDays,
                archivedRetentionDays))
            .Where(candidate => candidate is not null)
            .Cast<ArchiveRetentionCandidateDto>()
            .OrderByDescending(x => x.FileSizeBytes)
            .ThenBy(x => x.RecordedAt)
            .Take(take)
            .ToList();

        return new ArchiveRetentionPreviewDto
        {
            GeneratedAt = now,
            MotionVideoRetentionDays = motionRetentionDays,
            NoMotionVideoRetentionDays = noMotionRetentionDays,
            ArchivedVideoRetentionDays = archivedRetentionDays,
            CandidateCount = candidates.Count,
            ReclaimableBytes = candidates.Sum(x => x.FileSizeBytes),
            Candidates = candidates
        };
    }

    private static ArchiveRetentionCandidateDto? BuildCandidate(
        Guid id,
        Guid cameraMarkId,
        string videoPath,
        string? thumbnailPath,
        DateTimeOffset recordedAt,
        long fileSizeBytes,
        bool hasMotionDetected,
        bool isArchived,
        DateTimeOffset now,
        int motionRetentionDays,
        int noMotionRetentionDays,
        int archivedRetentionDays)
    {
        var ageDays = Math.Max(0, (int)Math.Floor((now - recordedAt).TotalDays));
        var retentionDays = ResolveRetentionDays(hasMotionDetected, isArchived, motionRetentionDays, noMotionRetentionDays, archivedRetentionDays);
        if (ageDays < retentionDays)
        {
            return null;
        }

        return new ArchiveRetentionCandidateDto
        {
            VideoArchiveId = id,
            CameraMarkId = cameraMarkId,
            VideoPath = videoPath,
            ThumbnailPath = thumbnailPath,
            RecordedAt = recordedAt,
            FileSizeBytes = fileSizeBytes,
            HasMotionDetected = hasMotionDetected,
            IsArchived = isArchived,
            AgeDays = ageDays,
            RetentionDays = retentionDays,
            Reason = ResolveReason(hasMotionDetected, isArchived)
        };
    }

    private static int ResolveRetentionDays(
        bool hasMotionDetected,
        bool isArchived,
        int motionRetentionDays,
        int noMotionRetentionDays,
        int archivedRetentionDays)
    {
        if (isArchived)
        {
            return archivedRetentionDays;
        }

        return hasMotionDetected ? motionRetentionDays : noMotionRetentionDays;
    }

    private static string ResolveReason(bool hasMotionDetected, bool isArchived)
    {
        if (isArchived)
        {
            return "Archived video exceeded archived retention policy.";
        }

        return hasMotionDetected
            ? "Motion video exceeded motion retention policy."
            : "No-motion video exceeded short retention policy.";
    }
}

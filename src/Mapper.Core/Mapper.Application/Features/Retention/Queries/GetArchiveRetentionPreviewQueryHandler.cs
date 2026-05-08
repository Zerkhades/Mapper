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
            .Select(video => ArchiveRetentionPolicy.BuildCandidate(
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
}

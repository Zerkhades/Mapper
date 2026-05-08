using Mapper.Application.Features.Retention.DTOs;
using Mapper.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.Retention.Commands;

public class CleanupArchiveRetentionCommandHandler
    : IRequestHandler<CleanupArchiveRetentionCommand, ArchiveRetentionCleanupResultDto>
{
    private readonly IMapperDbContext _db;
    private readonly IS3ObjectStorage _storage;

    public CleanupArchiveRetentionCommandHandler(IMapperDbContext db, IS3ObjectStorage storage)
    {
        _db = db;
        _storage = storage;
    }

    public async Task<ArchiveRetentionCleanupResultDto> Handle(
        CleanupArchiveRetentionCommand request,
        CancellationToken ct)
    {
        if (!request.DryRun && !request.Confirm)
        {
            throw new InvalidOperationException("Archive retention cleanup requires Confirm=true when DryRun=false.");
        }

        var now = request.Now ?? DateTimeOffset.UtcNow;
        var motionRetentionDays = Math.Max(1, request.MotionVideoRetentionDays);
        var noMotionRetentionDays = Math.Max(1, request.NoMotionVideoRetentionDays);
        var archivedRetentionDays = Math.Max(1, request.ArchivedVideoRetentionDays);
        var take = Math.Clamp(request.Take, 1, 500);

        var query = _db.CameraVideoArchives.AsQueryable();
        if (request.CameraMarkId.HasValue)
        {
            query = query.Where(x => x.CameraMarkId == request.CameraMarkId.Value);
        }

        var videos = await query.ToListAsync(ct);
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

        var deletedCount = 0;
        if (!request.DryRun)
        {
            var candidateIds = candidates.Select(x => x.VideoArchiveId).ToHashSet();
            foreach (var video in videos.Where(x => candidateIds.Contains(x.Id)))
            {
                await _storage.DeleteAsync(video.VideoPath, ct);
                if (!string.IsNullOrWhiteSpace(video.ThumbnailPath))
                {
                    await _storage.DeleteAsync(video.ThumbnailPath, ct);
                }

                _db.CameraVideoArchives.Remove(video);
                deletedCount++;
            }

            await _db.SaveChangesAsync(ct);
        }

        return new ArchiveRetentionCleanupResultDto
        {
            ExecutedAt = now,
            DryRun = request.DryRun,
            Confirmed = request.Confirm,
            CandidateCount = candidates.Count,
            DeletedCount = deletedCount,
            ReclaimableBytes = candidates.Sum(x => x.FileSizeBytes),
            Candidates = candidates
        };
    }
}

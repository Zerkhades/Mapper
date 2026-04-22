using Mapper.Application.Features.CameraArchive.Commands;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using Mapper.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mapper.Infrastructure.BackgroundJobs;

public class RecordCameraVideoJob
{
    private const int MaxParallelCameras = 2;

    private readonly MapperDbContext _db;
    private readonly ICameraAdapter _adapter;
    private readonly IS3ObjectStorage _storage;
    private readonly IMediator _mediator;
    private readonly ILogger<RecordCameraVideoJob> _logger;
    private readonly int _recordDurationSeconds;

    public RecordCameraVideoJob(
        MapperDbContext db,
        ICameraAdapter adapter,
        IS3ObjectStorage storage,
        IMediator mediator,
        ILogger<RecordCameraVideoJob> logger,
        int recordDurationSeconds = 300) // Default 5 minutes
    {
        _db = db;
        _adapter = adapter;
        _storage = storage;
        _mediator = mediator;
        _logger = logger;
        _recordDurationSeconds = recordDurationSeconds;
    }

    public async Task Execute(CancellationToken ct = default)
    {
        var cameras = await _db.GeoMarks
            .OfType<CameraMark>()
            .AsNoTracking()
            .Select(c => new { c.Id, c.StreamUrl })
            .ToListAsync(ct);

        await Parallel.ForEachAsync(cameras, new ParallelOptions
        {
            CancellationToken = ct,
            MaxDegreeOfParallelism = MaxParallelCameras
        }, async (cam, ct) =>
        {
            try
            {
                var recordDuration = TimeSpan.FromSeconds(_recordDurationSeconds);

                // Try to get video from camera
                var video = await _adapter.TryGetVideoAsync(cam.StreamUrl, recordDuration, ct);
                if (video is null)
                    return;

                // Save video to storage
                var videoKey = $"cameras/{cam.Id}/videos/{DateTime.UtcNow:yyyy-MM-dd-HH-mm-ss}.mp4";
                await using var ms = new MemoryStream(video.Bytes);
                await _storage.PutAsync(videoKey, ms, video.ContentType, ct);

                // Generate thumbnail from first frame
                var snapshot = await _adapter.TryGetSnapshotAsync(cam.StreamUrl, ct);
                string? thumbnailPath = null;
                if (snapshot is not null)
                {
                    var thumbnailKey = $"cameras/{cam.Id}/thumbnails/{DateTime.UtcNow:yyyy-MM-dd-HH-mm-ss}.png";
                    await using var thumbMs = new MemoryStream(snapshot.Bytes);
                    await _storage.PutAsync(thumbnailKey, thumbMs, snapshot.ContentType, ct);
                    thumbnailPath = thumbnailKey;
                }

                // Create video archive record
                var videoArchiveId = await _mediator.Send(new CreateCameraVideoArchiveCommand(
                    cam.Id,
                    videoKey,
                    video.Duration,
                    video.Bytes.LongLength,
                    "1920x1080", // Default resolution - should be configurable
                    30, // Default FPS - should be configurable
                    thumbnailPath
                ), ct);

                // Detect motion in the recorded video
                var motionResult = await _adapter.TryDetectMotionAsync(cam.StreamUrl, snapshot?.Bytes ?? Array.Empty<byte>(), ct);
                if (motionResult?.HasMotion ?? false)
                {
                    // Update video archive with motion detection
                    await _mediator.Send(new UpdateVideoArchiveMotionDetectionCommand(
                        videoArchiveId,
                        true
                    ), ct);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Video recording failed for camera {CameraId}", cam.Id);
            }
        });
    }
}

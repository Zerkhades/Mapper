using Mapper.Application.Features.CameraArchive.Commands;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using Mapper.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mapper.Infrastructure.BackgroundJobs;

public class DetectCameraMotionJob
{
    private const int MaxParallelCameras = 4;

    private readonly MapperDbContext _db;
    private readonly ICameraAdapter _adapter;
    private readonly IS3ObjectStorage _storage;
    private readonly ILogger<DetectCameraMotionJob> _logger;
    private readonly IMediator _mediator;

    public DetectCameraMotionJob(
        MapperDbContext db,
        ICameraAdapter adapter,
        IS3ObjectStorage storage,
        IMediator mediator,
        ILogger<DetectCameraMotionJob> logger)
    {
        _db = db;
        _adapter = adapter;
        _storage = storage;
        _logger = logger;
        _mediator = mediator;

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
                // Get current snapshot
                var snapshot = await _adapter.TryGetSnapshotAsync(cam.StreamUrl, ct);
                if (snapshot is null)
                    return;
                // Detect motion
                var motionResult = await _adapter.TryDetectMotionAsync(cam.StreamUrl, snapshot.Bytes, ct);
                if (motionResult is null)
                    return;

                if (motionResult.HasMotion && motionResult.MotionPercentage > 15)
                {
                    var severity = motionResult.MotionPercentage switch
                    {
                        > 50 => MotionSeverity.High,
                        > 30 => MotionSeverity.Medium,
                        _ => MotionSeverity.Low
                    };

                    // Save snapshot
                    string? snapshotPath = null;
                    if (snapshot.Bytes.Length > 0)
                    {
                        var key = $"cameras/{cam.Id}/motion/{Guid.NewGuid()}.png";
                        await using var ms = new MemoryStream(snapshot.Bytes);
                        await _storage.PutAsync(key, ms, snapshot.ContentType, ct);
                        snapshotPath = key;
                    }

                    await _mediator.Send(new CreateCameraMotionAlertCommand(
                        cam.Id,
                        severity,
                        motionResult.MotionPercentage,
                        snapshotPath
                    ), ct);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Motion detection failed for camera {CameraId}", cam.Id);
            }
        });
    }
}

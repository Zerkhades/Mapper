using Mapper.Application.Interfaces;
using Mapper.Domain;
using Mapper.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mapper.Infrastructure.BackgroundJobs;

public class FetchCameraSnapshotsJob
{
    private const int MaxParallelCameras = 4;

    private readonly MapperDbContext _db;
    private readonly ICameraAdapter _adapter;
    private readonly IS3ObjectStorage _storage;
    private readonly ICacheService _cache;
    private readonly IMapRealtimeNotifier _notifier;
    private readonly ILogger<FetchCameraSnapshotsJob> _logger;

    public FetchCameraSnapshotsJob(
        MapperDbContext db,
        ICameraAdapter adapter,
        IS3ObjectStorage storage,
        ICacheService cache,
        IMapRealtimeNotifier notifier,
        ILogger<FetchCameraSnapshotsJob> logger)
    {
        _db = db;
        _adapter = adapter;
        _storage = storage;
        _cache = cache;
        _notifier = notifier;
        _logger = logger;
    }

    public async Task Execute(CancellationToken ct = default)
    {
        var cameras = await _db.GeoMarks
            .OfType<CameraMark>()
            .AsNoTracking()
            .Select(c => new { c.Id, c.GeoMapId, c.StreamUrl })
            .ToListAsync(ct);

        await Parallel.ForEachAsync(cameras, new ParallelOptions
        {
            CancellationToken = ct,
            MaxDegreeOfParallelism = MaxParallelCameras
        }, async (cam, ct) =>
        {
            try
            {
                var snap = await _adapter.TryGetSnapshotAsync(cam.StreamUrl, ct);
                if (snap is null)
                    return;

                var key = $"cameras/{cam.Id}/latest.png";

                await using var ms = new MemoryStream(snap.Bytes);
                await _storage.PutAsync(key, ms, snap.ContentType, ct);

                await _cache.SetAsync($"camera:{cam.Id}:snapshotKey", key, TimeSpan.FromHours(12), ct);

                await _notifier.MarkUpdated(cam.GeoMapId, new
                {
                    id = cam.Id,
                    snapshotUrl = $"/api/files/{key}"
                }, ct);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to refresh snapshot for camera {CameraId}", cam.Id);
            }
        });
    }
}

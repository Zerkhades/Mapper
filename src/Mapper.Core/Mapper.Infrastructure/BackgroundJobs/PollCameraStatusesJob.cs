using Mapper.Application.Interfaces;
using Mapper.Domain;
using Mapper.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mapper.Infrastructure.BackgroundJobs;

public class PollCameraStatusesJob
{
    private const int MaxParallelCameras = 8;

    private readonly MapperDbContext _db;
    private readonly ICameraAdapter _adapter;
    private readonly ICacheService _cache;
    private readonly IMapRealtimeNotifier _notifier;
    private readonly ILogger<PollCameraStatusesJob> _logger;

    public PollCameraStatusesJob(
        MapperDbContext db,
        ICameraAdapter adapter,
        ICacheService cache,
        IMapRealtimeNotifier notifier,
        ILogger<PollCameraStatusesJob> logger)
    {
        _db = db;
        _adapter = adapter;
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
                var status = await _adapter.GetStatusAsync(cam.StreamUrl, ct);
                var newValue = status.IsOnline ? "online" : "offline";

                var key = $"camera:{cam.Id}:status";
                var oldValue = await _cache.GetAsync<string>(key, ct);

                if (string.Equals(oldValue, newValue, StringComparison.OrdinalIgnoreCase))
                    return;

                await _cache.SetAsync(key, newValue, TimeSpan.FromHours(12), ct);
                await _notifier.CameraStatusChanged(cam.GeoMapId, cam.Id, newValue, ct);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to poll status for camera {CameraId}", cam.Id);
            }
        });
    }
}

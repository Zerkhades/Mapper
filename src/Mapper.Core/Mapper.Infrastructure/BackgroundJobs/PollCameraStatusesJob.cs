using Mapper.Application.Interfaces;
using Mapper.Domain;
using Mapper.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Infrastructure.BackgroundJobs;

public class PollCameraStatusesJob
{
    private readonly MapperDbContext _db;
    private readonly ICameraAdapter _adapter;
    private readonly ICacheService _cache;
    private readonly IMapRealtimeNotifier _notifier;

    public PollCameraStatusesJob(
        MapperDbContext db,
        ICameraAdapter adapter,
        ICacheService cache,
        IMapRealtimeNotifier notifier)
    {
        _db = db;
        _adapter = adapter;
        _cache = cache;
        _notifier = notifier;
    }

    public async Task Execute(CancellationToken ct = default)
    {
        var cameras = await _db.GeoMarks
            .OfType<CameraMark>()
            .AsNoTracking()
            .Select(c => new { c.Id, c.GeoMapId, c.StreamUrl })
            .ToListAsync(ct);

        foreach (var cam in cameras)
        {
            var status = await _adapter.GetStatusAsync(cam.StreamUrl, ct);
            var newValue = status.IsOnline ? "online" : "offline";

            var key = $"camera:{cam.Id}:status";
            var oldValue = await _cache.GetAsync<string>(key, ct);

            if (string.Equals(oldValue, newValue, StringComparison.OrdinalIgnoreCase))
                continue;

            await _cache.SetAsync(key, newValue, TimeSpan.FromHours(12), ct);

            await _notifier.CameraStatusChanged(cam.GeoMapId, cam.Id, newValue, ct);
        }
    }
}

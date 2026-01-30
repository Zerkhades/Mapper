using Mapper.Application.Features.CameraArchive.Commands;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using Mapper.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Infrastructure.BackgroundJobs;

public class PollCameraStatusAndLogHistoryJob
{
    private readonly MapperDbContext _db;
    private readonly ICameraAdapter _adapter;
    private readonly ICacheService _cache;
    private readonly IMapRealtimeNotifier _notifier;
    private readonly IMediator _mediator;

    public PollCameraStatusAndLogHistoryJob(
        MapperDbContext db,
        ICameraAdapter adapter,
        ICacheService cache,
        IMapRealtimeNotifier notifier,
        IMediator mediator)
    {
        _db = db;
        _adapter = adapter;
        _cache = cache;
        _notifier = notifier;
        _mediator = mediator;
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
            try
            {
                var status = await _adapter.GetStatusAsync(cam.StreamUrl, ct);
                var newValue = status.IsOnline ? "online" : "offline";

                var cacheKey = $"camera:{cam.Id}:status";
                var oldValue = await _cache.GetAsync<string>(cacheKey, ct);

                if (string.Equals(oldValue, newValue, StringComparison.OrdinalIgnoreCase))
                    continue;

                // Status changed, create history record
                var reason = newValue == "online" 
                    ? CameraStatusReason.NetworkConnected 
                    : CameraStatusReason.NetworkDisconnected;

                await _mediator.Send(new CreateCameraStatusHistoryCommand(
                    cam.Id,
                    status.IsOnline,
                    reason,
                    status.Message,
                    status.RttMs
                ), ct);

                await _cache.SetAsync(cacheKey, newValue, TimeSpan.FromHours(12), ct);

                await _notifier.CameraStatusChanged(cam.GeoMapId, cam.Id, newValue, ct);
            }
            catch (Exception ex)
            {
                // Log error but continue
                System.Console.WriteLine($"Status polling error for camera {cam.Id}: {ex.Message}");
            }
        }
    }
}

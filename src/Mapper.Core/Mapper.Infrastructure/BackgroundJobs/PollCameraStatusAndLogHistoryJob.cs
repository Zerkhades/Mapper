using Mapper.Application.Features.CameraArchive.Commands;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using Mapper.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mapper.Infrastructure.BackgroundJobs;

public class PollCameraStatusAndLogHistoryJob
{
    private const int MaxParallelCameras = 8;

    private readonly MapperDbContext _db;
    private readonly ICameraAdapter _adapter;
    private readonly ICacheService _cache;
    private readonly IMapRealtimeNotifier _notifier;
    private readonly IMediator _mediator;
    private readonly ILogger<PollCameraStatusAndLogHistoryJob> _logger;

    public PollCameraStatusAndLogHistoryJob(
        MapperDbContext db,
        ICameraAdapter adapter,
        ICacheService cache,
        IMapRealtimeNotifier notifier,
        IMediator mediator,
        ILogger<PollCameraStatusAndLogHistoryJob> logger)
    {
        _db = db;
        _adapter = adapter;
        _cache = cache;
        _notifier = notifier;
        _mediator = mediator;
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

                var cacheKey = $"camera:{cam.Id}:status";
                var oldValue = await _cache.GetAsync<string>(cacheKey, ct);

                if (string.Equals(oldValue, newValue, StringComparison.OrdinalIgnoreCase))
                    return;

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
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Status polling with history failed for camera {CameraId}", cam.Id);
            }
        });
    }
}

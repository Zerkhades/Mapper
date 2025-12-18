using Microsoft.AspNetCore.SignalR;
using Mapper.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Infrastructure.Realtime
{
    public class MapRealtimeNotifier : IMapRealtimeNotifier
    {
        private readonly IHubContext<MapHub> _hub;

        public MapRealtimeNotifier(IHubContext<MapHub> hub) => _hub = hub;

        public Task MarkAdded(Guid geoMapId, object payload, CancellationToken ct)
            => _hub.Clients.Group(MapHub.Group(geoMapId.ToString()))
                .SendAsync("MarkAdded", payload, ct);

        public Task MarkUpdated(Guid geoMapId, object payload, CancellationToken ct)
            => _hub.Clients.Group(MapHub.Group(geoMapId.ToString()))
                .SendAsync("MarkUpdated", payload, ct);

        public Task MarkDeleted(Guid geoMapId, Guid markId, CancellationToken ct)
            => _hub.Clients.Group(MapHub.Group(geoMapId.ToString()))
                .SendAsync("MarkDeleted", new { markId }, ct);

        public Task CameraStatusChanged(Guid geoMapId, Guid markId, string status, CancellationToken ct)
            => _hub.Clients.Group(MapHub.Group(geoMapId.ToString()))
                .SendAsync("CameraStatusChanged", new { markId, status }, ct);
    }
}

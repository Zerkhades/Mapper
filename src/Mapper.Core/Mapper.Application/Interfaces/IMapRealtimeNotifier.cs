using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Application.Interfaces
{
    public interface IMapRealtimeNotifier
    {
        Task MarkAdded(Guid geoMapId, object payload, CancellationToken ct);
        Task MarkUpdated(Guid geoMapId, object payload, CancellationToken ct);
        Task MarkDeleted(Guid geoMapId, Guid markId, CancellationToken ct);

        Task CameraStatusChanged(Guid geoMapId, Guid markId, string status, CancellationToken ct);
    }
}

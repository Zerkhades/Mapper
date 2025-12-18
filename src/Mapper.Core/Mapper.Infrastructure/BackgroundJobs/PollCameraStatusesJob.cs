using Mapper.Application.Interfaces;
using Mapper.Domain;
using Mapper.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Infrastructure.BackgroundJobs
{
    public class PollCameraStatusesJob
    {
        private readonly MapperDbContext _db;
        private readonly ICameraAdapter _adapter;
        private readonly IMapRealtimeNotifier _notifier;

        public PollCameraStatusesJob(MapperDbContext db, ICameraAdapter adapter, IMapRealtimeNotifier notifier)
        {
            _db = db; _adapter = adapter; _notifier = notifier;
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
                var online = await _adapter.IsOnlineAsync(cam.StreamUrl, ct);
                var status = online ? "online" : "offline";

                await _notifier.CameraStatusChanged(cam.GeoMapId, cam.Id, status, ct);
            }
        }
    }
}

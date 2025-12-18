using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Application.Features.GeoMaps
{
    public class CreateGeoMapHandler : IRequestHandler<CreateGeoMapCommand, Guid>
    {
        private readonly IMapperDbContext _db;
        private readonly IMapImageStorage _storage;

        public CreateGeoMapHandler(IMapperDbContext db, IMapImageStorage storage)
        {
            _db = db; _storage = storage;
        }

        public async Task<Guid> Handle(CreateGeoMapCommand request, CancellationToken ct)
        {
            var path = await _storage.SaveAsync(request.ImageStream, request.FileName, request.ContentType, ct);

            var map = new GeoMap(request.Name, path, request.ImageWidth, request.ImageHeight, request.Description);

            _db.GeoMaps.Add(map);
            await _db.SaveChangesAsync(ct);

            return map.Id;
        }
    }

}

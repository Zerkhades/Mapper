using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Commands.CreateGeoMapCommand
{
    public class CreateGeoMapCommandHandler
        : IRequestHandler<CreateGeoMapCommand, Guid>
    {
        private readonly IMapperDbContext _dbContext;

        public CreateGeoMapCommandHandler(IMapperDbContext dbContext) =>
            _dbContext = dbContext;

        public async Task<Guid> Handle(CreateGeoMapCommand request,
            CancellationToken cancellationToken)
        {
            var map = new Domain.GeoMap
            {
                Id = Guid.NewGuid(),
                MapName = request.MapName,
                MapDescription = request.MapDescription,
                //Map = new byte[]
                //{
                //},
                IsArchived = false,
                //GeoMarks = null
            };

            await _dbContext.GeoMaps.AddAsync(map, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return map.Id;
        }
    }
}

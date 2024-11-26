using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapper.Application.Common.Exceptions;

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
            var entity = await _dbContext.GeoMaps
                .FindAsync(new object[] { request.Id }, cancellationToken);
            if (entity != null)
            {
                throw new AlreadyExistsException(nameof(Domain.GeoMap), request.Id);
            }
            var map = new Domain.GeoMap
            {
                Id = Guid.NewGuid(),
                MapName = request.MapName,
                MapDescription = request.MapDescription,
                //Add new class with photo Map parameter
                IsArchived = false,
                GeoMarks = request.GeoMarks,
            };

            await _dbContext.GeoMaps.AddAsync(map, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return map.Id;
        }
    }
}

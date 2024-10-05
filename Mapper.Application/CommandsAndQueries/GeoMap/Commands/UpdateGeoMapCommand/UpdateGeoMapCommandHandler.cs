using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapper.Application.Interfaces;
using Mapper.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Mapper.Domain;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Commands.UpdateGeoMapCommand
{
    public class UpdateGeoMapCommandHandler
        : IRequestHandler<UpdateGeoMapCommand>
    {
        private readonly IMapperDbContext _dbContext;

        public UpdateGeoMapCommandHandler(IMapperDbContext dbContext) =>
            _dbContext = dbContext;

        public async Task Handle(UpdateGeoMapCommand request, CancellationToken cancellationToken)
        {
            var entity =
                await _dbContext.GeoMaps.FirstOrDefaultAsync(geoMap =>
                    geoMap.Id == request.Id, cancellationToken);

            if (entity == null || entity.Id != request.Id)
            {
                throw new NotFoundException(nameof(GeoMap), request.Id);
            }

            entity.MapName = request.MapName;
            entity.MapDescription = request.MapDescription;
            //entity.Map = request.Map;
            entity.Id = request.Id;

            await _dbContext.SaveChangesAsync(cancellationToken);


        }
    }
}

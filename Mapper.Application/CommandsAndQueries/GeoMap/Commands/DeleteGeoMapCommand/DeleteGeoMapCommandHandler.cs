using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Mapper.Application.Interfaces;
using Mapper.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Commands.DeleteGeoMapCommand
{
    public class DeleteGeoMapCommandHandler : IRequestHandler<DeleteGeoMapCommand>
    {
        private readonly IMapperDbContext _dbContext;

        public DeleteGeoMapCommandHandler(IMapperDbContext dbContext) =>
            _dbContext = dbContext;

        public async Task Handle(DeleteGeoMapCommand request, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.GeoMaps.FirstOrDefaultAsync(geoMap =>
                geoMap.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(GeoMap), request.Id);
            }

            _dbContext.GeoMaps.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
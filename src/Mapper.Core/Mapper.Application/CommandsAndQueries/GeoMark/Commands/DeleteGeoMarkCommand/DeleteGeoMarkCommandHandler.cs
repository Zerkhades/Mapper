using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.CommandsAndQueries.GeoMark.Commands.DeleteGeoMarkCommand
{
    public class DeleteGeoMarkCommandHandler : IRequestHandler<DeleteGeoMarkCommand>
    {
        private readonly IMapperDbContext _dbContext;

        public DeleteGeoMarkCommandHandler(IMapperDbContext dbContext) =>
            _dbContext = dbContext;

        public async Task Handle(DeleteGeoMarkCommand request, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.GeoMarks.FirstOrDefaultAsync(geoMark =>
                geoMark.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(GeoMark), request.Id);
            }

            _dbContext.GeoMarks.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

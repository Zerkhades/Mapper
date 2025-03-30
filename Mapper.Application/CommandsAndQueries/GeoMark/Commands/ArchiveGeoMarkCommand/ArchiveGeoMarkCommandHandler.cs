using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using MediatR;

namespace Mapper.Application.CommandsAndQueries.GeoMark.Commands.ArchiveGeoMarkCommand
{
    public class ArchiveGeoMarkCommandHandler
    : IRequestHandler<ArchiveGeoMarkCommand>
    {
        private readonly IMapperDbContext _dbContext;
        public ArchiveGeoMarkCommandHandler(IMapperDbContext dbContext) =>
            _dbContext = dbContext;

        public async Task Handle(ArchiveGeoMarkCommand request, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.GeoMarks
                .FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null /*|| entity.UserId != request.UserId*/)
            {
                throw new NotFoundException(nameof(Domain.GeoMark), request.Id);
            }

            entity.IsArchived = true;
            await _dbContext.SaveChangesAsync(cancellationToken);

        }
    }
}

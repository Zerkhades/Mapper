using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using MediatR;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Commands.DeleteGeoMapCommand
{
    public class DeleteGeoMapCommandHandler
        : IRequestHandler<DeleteGeoMapCommand>
    {
        private readonly IMapperDbContext _dbContext;

        public DeleteGeoMapCommandHandler(IMapperDbContext dbContext) =>
            _dbContext = dbContext;


        public async Task Handle(DeleteGeoMapCommand request, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.GeoMaps
                .FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null /*|| entity.UserId != request.UserId*/)
            {
                throw new NotFoundException(nameof(Domain.GeoMap), request.Id);
            }

            entity.IsArchived = true;
            await _dbContext.SaveChangesAsync(cancellationToken);

        }
    }
}

using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using MediatR;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Commands.DeleteGeoMapCommand
{
    public class DeleteGeoMapHandler
        : IRequestHandler<DeleteGeoMapCommand>
    {
        private readonly IMapperDbContext _dbContext;

        public DeleteGeoMapHandler(IMapperDbContext dbContext) =>
            _dbContext = dbContext;


        public async Task Handle(DeleteGeoMapCommand request, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.GeoMarks
                .FindAsync(new object[] { request.Id }, cancellationToken);
            entity.IsArchived = true;
            await _dbContext.SaveChangesAsync(cancellationToken);

        }
    }
}

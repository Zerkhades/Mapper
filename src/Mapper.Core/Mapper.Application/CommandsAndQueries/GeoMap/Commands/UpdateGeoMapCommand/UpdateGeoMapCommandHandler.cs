using MediatR;
using Mapper.Application.Interfaces;
using Mapper.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Commands.UpdateGeoMapCommand
{
    //public class UpdateGeoMapCommandHandler
    //    : IRequestHandler<UpdateGeoMapCommand>
    //{
    //    private readonly IMapperDbContext _dbContext;

    //    public UpdateGeoMapCommandHandler(IMapperDbContext dbContext) =>
    //        _dbContext = dbContext;

    //    public async Task Handle(UpdateGeoMapCommand request, CancellationToken cancellationToken)
    //    {
    //        var entity =
    //            await _dbContext.GeoMaps.FirstOrDefaultAsync(geoMap =>
    //                geoMap.Id == request.Id, cancellationToken);

    //        if (entity == null)
    //        {
    //            throw new NotFoundException(nameof(GeoMap), request.Id);
    //        }
    //        entity.Id = request.Id;
    //        entity.MapName = request.MapName;
    //        entity.MapDescription = request.MapDescription;
    //        entity.GeoMarks = request.GeoMarks;
    //        entity.IsArchived = request.IsArchived;
    //        //entity.Map = request.Map;


    //        await _dbContext.SaveChangesAsync(cancellationToken);


    //    }
    //}
}

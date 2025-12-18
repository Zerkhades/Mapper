using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Mapper.Application.CommandsAndQueries.GeoMark.Commands.UpdateGeoMarkCommand
{
    //public class UpdateGeoMarkCommandHandler
    //            : IRequestHandler<UpdateGeoMarkCommand>
    //{
    //    private readonly IMapperDbContext _dbContext;

    //    public UpdateGeoMarkCommandHandler(IMapperDbContext dbContext) =>
    //        _dbContext = dbContext;

    //    public async Task Handle(UpdateGeoMarkCommand request, CancellationToken cancellationToken)
    //    {
    //        var entity =
    //            await _dbContext.GeoMarks.FirstOrDefaultAsync(geoMark =>
    //                geoMark.Id == request.Id, cancellationToken);

    //        if (entity == null)
    //        {
    //            throw new NotFoundException(nameof(GeoMark), request.Id);
    //        }
    //        entity.Id = request.Id;
    //        entity.MarkName = request.MarkName;
    //        entity.MarkDescription = request.MarkDescription;
    //        entity.Color = request.Color;
    //        entity.Emoji = request.Emoji;
    //        entity.Size = request.Size;
    //        entity.IsEmoji = request.IsEmoji;
    //        entity.IsArchived = request.IsArchived;
    //        entity.IsEditable = request.IsEditable;
    //        entity.EditDate = request.EditDate;
    //        entity.EditedBy = request.EditedBy;

    //        await _dbContext.SaveChangesAsync(cancellationToken);
    //    }
    //}
}

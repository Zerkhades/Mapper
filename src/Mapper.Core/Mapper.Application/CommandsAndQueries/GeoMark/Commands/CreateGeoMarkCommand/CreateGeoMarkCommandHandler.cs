using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using MediatR;

namespace Mapper.Application.CommandsAndQueries.GeoMark.Commands.CreateGeoMarkCommand
{
    //public class CreateGeoMarkCommandHandler
    //: IRequestHandler<CreateGeoMarkCommand, Guid>
    //{
    //    private readonly IMapperDbContext _dbContext;
    //    public CreateGeoMarkCommandHandler(IMapperDbContext dbContext) =>
    //        _dbContext = dbContext;

    //    public async Task<Guid> Handle(CreateGeoMarkCommand request,
    //        CancellationToken cancellationToken)
    //    {
    //        var entity = await _dbContext.GeoMarks
    //            .FindAsync(new object[] { request.Id }, cancellationToken);
    //        if (entity != null)
    //        {
    //            throw new AlreadyExistsException(nameof(Domain.GeoMark), request.Id);
    //        }

    //        var mark = new Domain.GeoMark
    //        {
    //            Id = new Guid(),
    //            XPos = request.XPos,
    //            YPos = request.YPos,
    //            GeoMapId = request.GeoMapId,
    //            GeoMap = request.GeoMap,
    //            MarkName = request.MarkName,
    //            MarkDescription = request.MarkDescription,
    //            Color = request.Color,
    //            Emoji = request.Emoji,
    //            Size = request.Size,
    //            IsEmoji = request.IsEmoji,
    //            IsArchived = request.IsArchived,
    //            IsEditable = request.IsEditable,
    //            CreationDate = request.CreationDate,
    //            EditDate = request.EditDate,
    //            EditedBy = request.EditedBy,
    //            Employees = request.Employees,
    //            GeoPhotos = request.GeoPhotos,
    //        };
    //        await _dbContext.GeoMarks.AddAsync(mark, cancellationToken);
    //        await _dbContext.SaveChangesAsync(cancellationToken);

    //        return mark.Id;
    //    }
    //}
}

using FluentValidation;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.GeoMarks.Commands.DeleteGeoMark;

public record DeleteGeoMarkCommand(Guid GeoMapId, Guid GeoMarkId) : IRequest;

public class DeleteGeoMarkValidator : AbstractValidator<DeleteGeoMarkCommand>
{
    public DeleteGeoMarkValidator()
    {
        RuleFor(x => x.GeoMapId).NotEmpty();
        RuleFor(x => x.GeoMarkId).NotEmpty();
    }
}

public class DeleteGeoMarkHandler : IRequestHandler<DeleteGeoMarkCommand>
{
    private readonly IMapperDbContext _db;

    public DeleteGeoMarkHandler(IMapperDbContext db) => _db = db;

    public async Task Handle(DeleteGeoMarkCommand request, CancellationToken ct)
    {
        var mark = await _db.GeoMarks
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == request.GeoMarkId && x.GeoMapId == request.GeoMapId, ct);

        if (mark is null)
            throw new NotFoundException($"GeoMark {request.GeoMarkId} not found", request.GeoMarkId);

        mark.SoftDelete();
        await _db.SaveChangesAsync(ct);
    }
}

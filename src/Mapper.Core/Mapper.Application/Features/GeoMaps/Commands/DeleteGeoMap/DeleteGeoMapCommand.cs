using FluentValidation;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.GeoMaps.Commands.DeleteGeoMap;

public record DeleteGeoMapCommand(Guid Id) : IRequest;

public class DeleteGeoMapValidator : AbstractValidator<DeleteGeoMapCommand>
{
    public DeleteGeoMapValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class DeleteGeoMapHandler : IRequestHandler<DeleteGeoMapCommand>
{
    private readonly IMapperDbContext _db;

    public DeleteGeoMapHandler(IMapperDbContext db) => _db = db;

    public async Task Handle(DeleteGeoMapCommand request, CancellationToken ct)
    {
        var map = await _db.GeoMaps
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (map is null)
            throw new NotFoundException($"GeoMap {request.Id} not found", request.Id);

        map.SoftDelete();
        await _db.SaveChangesAsync(ct);
    }
}

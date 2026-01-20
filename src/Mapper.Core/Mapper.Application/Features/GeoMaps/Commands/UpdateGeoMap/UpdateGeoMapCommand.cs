using FluentValidation;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.GeoMaps.Commands.UpdateGeoMap;

public record UpdateGeoMapCommand(
    Guid Id,
    string Name,
    string? Description
) : IRequest;

public class UpdateGeoMapValidator : AbstractValidator<UpdateGeoMapCommand>
{
    public UpdateGeoMapValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}

public class UpdateGeoMapHandler : IRequestHandler<UpdateGeoMapCommand>
{
    private readonly IMapperDbContext _db;
    private readonly ICacheService _cacheService;

    public UpdateGeoMapHandler(IMapperDbContext db, ICacheService cacheService)
    {
        _db = db;
        _cacheService = cacheService;
    }

    public async Task Handle(UpdateGeoMapCommand request, CancellationToken ct)
    {
        var map = await _db.GeoMaps
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (map is null)
            throw new NotFoundException($"GeoMap {request.Id} not found", request.Id);

        map.Update(request.Name, request.Description);
        await _db.SaveChangesAsync(ct);

        await _cacheService.RemoveAsync($"geomap:{request.Id}", ct);
    }
}

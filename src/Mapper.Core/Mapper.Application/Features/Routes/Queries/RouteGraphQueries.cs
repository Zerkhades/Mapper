using Mapper.Application.Common.Exceptions;
using Mapper.Application.Features.Routes.DTOs;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.Routes.Queries;

public record GetRouteNodesQuery(Guid GeoMapId) : IRequest<IReadOnlyList<RouteNodeDto>>;

public record GetRouteEdgesQuery(Guid GeoMapId) : IRequest<IReadOnlyList<RouteEdgeDto>>;

public sealed class GetRouteNodesQueryHandler : IRequestHandler<GetRouteNodesQuery, IReadOnlyList<RouteNodeDto>>
{
    private readonly IMapperDbContext _db;

    public GetRouteNodesQueryHandler(IMapperDbContext db) => _db = db;

    public async Task<IReadOnlyList<RouteNodeDto>> Handle(GetRouteNodesQuery request, CancellationToken ct)
    {
        await EnsureMapExists(request.GeoMapId, ct);

        return await _db.RouteNodes
            .AsNoTracking()
            .Where(x => x.GeoMapId == request.GeoMapId)
            .OrderBy(x => x.Title)
            .ThenBy(x => x.Id)
            .Select(x => new RouteNodeDto(x.Id, x.GeoMapId, x.GeoMarkId, x.X, x.Y, x.Title))
            .ToListAsync(ct);
    }

    private async Task EnsureMapExists(Guid geoMapId, CancellationToken ct)
    {
        if (!await _db.GeoMaps.AnyAsync(x => x.Id == geoMapId, ct))
            throw new NotFoundException(nameof(GeoMap), geoMapId);
    }
}

public sealed class GetRouteEdgesQueryHandler : IRequestHandler<GetRouteEdgesQuery, IReadOnlyList<RouteEdgeDto>>
{
    private readonly IMapperDbContext _db;

    public GetRouteEdgesQueryHandler(IMapperDbContext db) => _db = db;

    public async Task<IReadOnlyList<RouteEdgeDto>> Handle(GetRouteEdgesQuery request, CancellationToken ct)
    {
        await EnsureMapExists(request.GeoMapId, ct);

        return await _db.RouteEdges
            .AsNoTracking()
            .Where(x => x.GeoMapId == request.GeoMapId)
            .OrderBy(x => x.Id)
            .Select(x => new RouteEdgeDto(
                x.Id,
                x.GeoMapId,
                x.FromNodeId,
                x.ToNodeId,
                x.CostOverride,
                x.IsBidirectional,
                x.IsDisabled,
                x.Description))
            .ToListAsync(ct);
    }

    private async Task EnsureMapExists(Guid geoMapId, CancellationToken ct)
    {
        if (!await _db.GeoMaps.AnyAsync(x => x.Id == geoMapId, ct))
            throw new NotFoundException(nameof(GeoMap), geoMapId);
    }
}

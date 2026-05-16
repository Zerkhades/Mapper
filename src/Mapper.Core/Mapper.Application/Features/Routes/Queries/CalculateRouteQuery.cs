using FluentValidation;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Features.Routes.DTOs;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.Routes.Queries;

public record CalculateRouteQuery(
    Guid GeoMapId,
    RouteEndpointDto Start,
    RouteEndpointDto End,
    double WalkingSpeed = 1.4) : IRequest<CalculatedRouteDto>;

public class CalculateRouteQueryValidator : AbstractValidator<CalculateRouteQuery>
{
    public CalculateRouteQueryValidator()
    {
        RuleFor(x => x.WalkingSpeed).GreaterThan(0);
        RuleFor(x => x.Start).Must(HaveEndpoint).WithMessage("Start endpoint is required");
        RuleFor(x => x.End).Must(HaveEndpoint).WithMessage("End endpoint is required");
    }

    private static bool HaveEndpoint(RouteEndpointDto endpoint)
        => endpoint.NodeId is not null ||
            endpoint.GeoMarkId is not null ||
            (endpoint.X is not null && endpoint.Y is not null);
}

public sealed class CalculateRouteQueryHandler : IRequestHandler<CalculateRouteQuery, CalculatedRouteDto>
{
    private readonly IMapperDbContext _db;

    public CalculateRouteQueryHandler(IMapperDbContext db) => _db = db;

    public async Task<CalculatedRouteDto> Handle(CalculateRouteQuery request, CancellationToken ct)
    {
        if (!await _db.GeoMaps.AnyAsync(x => x.Id == request.GeoMapId, ct))
            throw new NotFoundException(nameof(GeoMap), request.GeoMapId);

        var nodes = await _db.RouteNodes
            .AsNoTracking()
            .Where(x => x.GeoMapId == request.GeoMapId)
            .ToListAsync(ct);

        if (nodes.Count == 0)
            return EmptyRoute(request.GeoMapId);

        var edges = await _db.RouteEdges
            .AsNoTracking()
            .Where(x => x.GeoMapId == request.GeoMapId && !x.IsDisabled)
            .ToListAsync(ct);

        var start = await ResolveEndpoint(request.GeoMapId, request.Start, nodes, ct);
        var end = await ResolveEndpoint(request.GeoMapId, request.End, nodes, ct);
        var path = RoutePathfinder.FindPath(nodes, edges, start.Node.Id, end.Node.Id);

        if (path.Count == 0)
            return EmptyRoute(request.GeoMapId);

        var points = new List<RoutePointDto>();
        AddPoint(points, start.X, start.Y);

        foreach (var node in path)
            AddPoint(points, node.X, node.Y);

        AddPoint(points, end.X, end.Y);

        var distance = CalculateDistance(points);
        var estimatedSeconds = (int)Math.Ceiling(distance / request.WalkingSpeed);

        return new CalculatedRouteDto(
            new[]
            {
                new RouteSegmentDto(request.GeoMapId, points, distance, estimatedSeconds)
            },
            distance,
            estimatedSeconds);
    }

    private async Task<ResolvedEndpoint> ResolveEndpoint(
        Guid geoMapId,
        RouteEndpointDto endpoint,
        IReadOnlyList<RouteNode> nodes,
        CancellationToken ct)
    {
        if (endpoint.NodeId is not null)
        {
            var node = nodes.FirstOrDefault(x => x.Id == endpoint.NodeId.Value);
            if (node is null)
                throw new NotFoundException(nameof(RouteNode), endpoint.NodeId.Value);

            return new ResolvedEndpoint(node, node.X, node.Y);
        }

        if (endpoint.GeoMarkId is not null)
        {
            var mark = await _db.GeoMarks
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == endpoint.GeoMarkId.Value && x.GeoMapId == geoMapId, ct);

            if (mark is null)
                throw new NotFoundException(nameof(GeoMark), endpoint.GeoMarkId.Value);

            var linkedNode = nodes.FirstOrDefault(x => x.GeoMarkId == endpoint.GeoMarkId.Value);
            return new ResolvedEndpoint(linkedNode ?? FindNearestNode(nodes, mark.X, mark.Y), mark.X, mark.Y);
        }

        var x = endpoint.X!.Value;
        var y = endpoint.Y!.Value;
        return new ResolvedEndpoint(FindNearestNode(nodes, x, y), x, y);
    }

    private static RouteNode FindNearestNode(IReadOnlyList<RouteNode> nodes, double x, double y)
        => nodes.MinBy(node => RoutePathfinder.Distance(node.X, node.Y, x, y))!;

    private static void AddPoint(List<RoutePointDto> points, double x, double y)
    {
        if (points.Count > 0)
        {
            var previous = points[^1];
            if (Math.Abs(previous.X - x) < double.Epsilon && Math.Abs(previous.Y - y) < double.Epsilon)
                return;
        }

        points.Add(new RoutePointDto(x, y));
    }

    private static double CalculateDistance(IReadOnlyList<RoutePointDto> points)
    {
        var distance = 0d;
        for (var i = 1; i < points.Count; i++)
            distance += RoutePathfinder.Distance(points[i - 1].X, points[i - 1].Y, points[i].X, points[i].Y);

        return distance;
    }

    private static CalculatedRouteDto EmptyRoute(Guid geoMapId)
        => new(new[] { new RouteSegmentDto(geoMapId, Array.Empty<RoutePointDto>(), 0, 0) }, 0, 0);

    private sealed record ResolvedEndpoint(RouteNode Node, double X, double Y);
}

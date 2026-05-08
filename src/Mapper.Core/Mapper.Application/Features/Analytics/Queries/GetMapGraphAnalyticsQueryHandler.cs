using Mapper.Application.Common.Exceptions;
using Mapper.Application.Features.Analytics.DTOs;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.Analytics.Queries;

public class GetMapGraphAnalyticsQueryHandler
    : IRequestHandler<GetMapGraphAnalyticsQuery, MapGraphAnalyticsDto>
{
    private readonly IMapperDbContext _db;

    public GetMapGraphAnalyticsQueryHandler(IMapperDbContext db)
    {
        _db = db;
    }

    public async Task<MapGraphAnalyticsDto> Handle(GetMapGraphAnalyticsQuery request, CancellationToken ct)
    {
        var top = Math.Clamp(request.Top, 1, 20);
        var maps = await _db.GeoMaps
            .AsNoTracking()
            .Select(x => new MapLookupItem(x.Id, x.Name))
            .ToListAsync(ct);

        if (request.SourceGeoMapId.HasValue && maps.All(x => x.Id != request.SourceGeoMapId.Value))
        {
            throw new NotFoundException("Source GeoMap", request.SourceGeoMapId.Value);
        }

        if (request.TargetGeoMapId.HasValue && maps.All(x => x.Id != request.TargetGeoMapId.Value))
        {
            throw new NotFoundException("Target GeoMap", request.TargetGeoMapId.Value);
        }

        var mapNames = maps.ToDictionary(x => x.Id, x => x.Name);
        var mapIds = maps.Select(x => x.Id).ToHashSet();

        var edges = await _db.GeoMarks
            .AsNoTracking()
            .OfType<TransitionMark>()
            .Select(x => new EdgeLookupItem(x.Id, x.GeoMapId, x.TargetGeoMapId, x.Title))
            .ToListAsync(ct);

        var adjacency = mapIds.ToDictionary(id => id, _ => new List<Guid>());
        var weakAdjacency = mapIds.ToDictionary(id => id, _ => new List<Guid>());

        foreach (var edge in edges.Where(x => mapIds.Contains(x.SourceGeoMapId) && mapIds.Contains(x.TargetGeoMapId)))
        {
            adjacency[edge.SourceGeoMapId].Add(edge.TargetGeoMapId);
            weakAdjacency[edge.SourceGeoMapId].Add(edge.TargetGeoMapId);
            weakAdjacency[edge.TargetGeoMapId].Add(edge.SourceGeoMapId);
        }

        var reachableByMap = mapIds.ToDictionary(id => id, id => Traverse(adjacency, id));
        var nodes = BuildNodes(maps, edges, reachableByMap, mapIds.Count);
        var weakComponents = BuildWeakComponents(weakAdjacency, mapNames);
        var shortestPaths = BuildShortestPaths(request, adjacency);

        return new MapGraphAnalyticsDto
        {
            MapCount = maps.Count,
            TransitionCount = edges.Count,
            IsolatedMapCount = nodes.Count(x => x.IsIsolated),
            WeakComponentCount = weakComponents.Count,
            DirectedReachabilityRatio = CalculateDirectedReachabilityRatio(reachableByMap, mapIds.Count),
            Nodes = nodes.OrderBy(x => x.Name).ToList(),
            Edges = edges.Select(edge => new MapGraphEdgeDto
            {
                TransitionMarkId = edge.TransitionMarkId,
                SourceGeoMapId = edge.SourceGeoMapId,
                SourceMapName = mapNames.GetValueOrDefault(edge.SourceGeoMapId, "Unknown source"),
                TargetGeoMapId = edge.TargetGeoMapId,
                TargetMapName = mapNames.GetValueOrDefault(edge.TargetGeoMapId),
                Title = edge.Title,
                TargetExists = mapIds.Contains(edge.TargetGeoMapId)
            }).ToList(),
            WeakComponents = weakComponents,
            Bottlenecks = nodes
                .Where(x => !x.IsIsolated)
                .OrderByDescending(x => x.ReachableMapCount)
                .ThenByDescending(x => x.IncomingTransitions + x.OutgoingTransitions)
                .Take(top)
                .ToList(),
            ShortestPaths = shortestPaths
        };
    }

    private static List<MapGraphNodeDto> BuildNodes(
        IReadOnlyCollection<MapLookupItem> maps,
        IReadOnlyCollection<EdgeLookupItem> edges,
        IReadOnlyDictionary<Guid, HashSet<Guid>> reachableByMap,
        int mapCount)
    {
        return maps.Select(map =>
        {
            var incoming = edges.Count(x => x.TargetGeoMapId == map.Id);
            var outgoing = edges.Count(x => x.SourceGeoMapId == map.Id);
            var reachableCount = reachableByMap[map.Id].Count;
            return new MapGraphNodeDto
            {
                GeoMapId = map.Id,
                Name = map.Name,
                IncomingTransitions = incoming,
                OutgoingTransitions = outgoing,
                ReachableMapCount = reachableCount,
                ReachabilityRatio = mapCount <= 1 ? 1 : Math.Round((double)reachableCount / (mapCount - 1), 2),
                IsIsolated = incoming == 0 && outgoing == 0
            };
        }).ToList();
    }

    private static List<MapGraphComponentDto> BuildWeakComponents(
        IReadOnlyDictionary<Guid, List<Guid>> weakAdjacency,
        IReadOnlyDictionary<Guid, string> mapNames)
    {
        var visited = new HashSet<Guid>();
        var components = new List<MapGraphComponentDto>();

        foreach (var mapId in weakAdjacency.Keys.OrderBy(id => mapNames[id]))
        {
            if (!visited.Add(mapId))
            {
                continue;
            }

            var component = Traverse(weakAdjacency, mapId, includeStart: true);
            foreach (var id in component)
            {
                visited.Add(id);
            }

            var orderedIds = component.OrderBy(id => mapNames[id]).ToList();
            components.Add(new MapGraphComponentDto
            {
                Index = components.Count + 1,
                MapCount = orderedIds.Count,
                GeoMapIds = orderedIds,
                MapNames = orderedIds.Select(id => mapNames[id]).ToList()
            });
        }

        return components
            .OrderByDescending(x => x.MapCount)
            .ThenBy(x => x.MapNames.FirstOrDefault())
            .ToList();
    }

    private static List<MapShortestPathDto> BuildShortestPaths(
        GetMapGraphAnalyticsQuery request,
        IReadOnlyDictionary<Guid, List<Guid>> adjacency)
    {
        if (!request.SourceGeoMapId.HasValue || !request.TargetGeoMapId.HasValue)
        {
            return [];
        }

        var path = FindShortestPath(adjacency, request.SourceGeoMapId.Value, request.TargetGeoMapId.Value);
        return
        [
            new MapShortestPathDto
            {
                SourceGeoMapId = request.SourceGeoMapId.Value,
                TargetGeoMapId = request.TargetGeoMapId.Value,
                Exists = path.Count > 0,
                Distance = path.Count == 0 ? -1 : path.Count - 1,
                Path = path
            }
        ];
    }

    private static HashSet<Guid> Traverse(
        IReadOnlyDictionary<Guid, List<Guid>> adjacency,
        Guid start,
        bool includeStart = false)
    {
        var visited = new HashSet<Guid>();
        var queue = new Queue<Guid>();
        queue.Enqueue(start);

        if (includeStart)
        {
            visited.Add(start);
        }

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (!adjacency.TryGetValue(current, out var neighbors))
            {
                continue;
            }

            foreach (var next in neighbors)
            {
                if (next == start && !includeStart)
                {
                    continue;
                }

                if (visited.Add(next))
                {
                    queue.Enqueue(next);
                }
            }
        }

        return visited;
    }

    private static List<Guid> FindShortestPath(
        IReadOnlyDictionary<Guid, List<Guid>> adjacency,
        Guid source,
        Guid target)
    {
        if (source == target)
        {
            return [source];
        }

        var visited = new HashSet<Guid> { source };
        var previous = new Dictionary<Guid, Guid>();
        var queue = new Queue<Guid>();
        queue.Enqueue(source);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (!adjacency.TryGetValue(current, out var neighbors))
            {
                continue;
            }

            foreach (var next in neighbors)
            {
                if (!visited.Add(next))
                {
                    continue;
                }

                previous[next] = current;
                if (next == target)
                {
                    return ReconstructPath(previous, source, target);
                }

                queue.Enqueue(next);
            }
        }

        return [];
    }

    private static List<Guid> ReconstructPath(
        IReadOnlyDictionary<Guid, Guid> previous,
        Guid source,
        Guid target)
    {
        var path = new List<Guid> { target };
        var current = target;

        while (current != source)
        {
            current = previous[current];
            path.Add(current);
        }

        path.Reverse();
        return path;
    }

    private static double CalculateDirectedReachabilityRatio(
        IReadOnlyDictionary<Guid, HashSet<Guid>> reachableByMap,
        int mapCount)
    {
        if (mapCount <= 1)
        {
            return 1;
        }

        var reachablePairs = reachableByMap.Sum(x => x.Value.Count);
        var possiblePairs = mapCount * (mapCount - 1);
        return Math.Round((double)reachablePairs / possiblePairs, 2);
    }

    private sealed record MapLookupItem(Guid Id, string Name);

    private sealed record EdgeLookupItem(
        Guid TransitionMarkId,
        Guid SourceGeoMapId,
        Guid TargetGeoMapId,
        string Title);
}

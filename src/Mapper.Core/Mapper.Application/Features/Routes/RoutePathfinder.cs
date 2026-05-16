using Mapper.Domain;

namespace Mapper.Application.Features.Routes;

public static class RoutePathfinder
{
    public static IReadOnlyList<RouteNode> FindPath(
        IReadOnlyList<RouteNode> nodes,
        IReadOnlyList<RouteEdge> edges,
        Guid startNodeId,
        Guid endNodeId)
    {
        var nodesById = nodes.ToDictionary(x => x.Id);
        if (!nodesById.ContainsKey(startNodeId) || !nodesById.ContainsKey(endNodeId))
            return Array.Empty<RouteNode>();

        var adjacency = BuildAdjacency(nodesById, edges);
        var open = new PriorityQueue<Guid, double>();
        var cameFrom = new Dictionary<Guid, Guid>();
        var gScore = nodesById.Keys.ToDictionary(x => x, _ => double.PositiveInfinity);

        gScore[startNodeId] = 0;
        open.Enqueue(startNodeId, Heuristic(nodesById[startNodeId], nodesById[endNodeId]));

        while (open.Count > 0)
        {
            var current = open.Dequeue();
            if (current == endNodeId)
                return ReconstructPath(cameFrom, nodesById, current);

            if (!adjacency.TryGetValue(current, out var neighbors))
                continue;

            foreach (var neighbor in neighbors)
            {
                var tentative = gScore[current] + neighbor.Cost;
                if (tentative >= gScore[neighbor.NodeId])
                    continue;

                cameFrom[neighbor.NodeId] = current;
                gScore[neighbor.NodeId] = tentative;
                var priority = tentative + Heuristic(nodesById[neighbor.NodeId], nodesById[endNodeId]);
                open.Enqueue(neighbor.NodeId, priority);
            }
        }

        return Array.Empty<RouteNode>();
    }

    public static double Distance(RouteNode from, RouteNode to)
        => Distance(from.X, from.Y, to.X, to.Y);

    public static double Distance(double fromX, double fromY, double toX, double toY)
    {
        var dx = fromX - toX;
        var dy = fromY - toY;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private static Dictionary<Guid, List<RouteNeighbor>> BuildAdjacency(
        IReadOnlyDictionary<Guid, RouteNode> nodesById,
        IReadOnlyList<RouteEdge> edges)
    {
        var adjacency = new Dictionary<Guid, List<RouteNeighbor>>();

        foreach (var edge in edges.Where(x => !x.IsDisabled))
        {
            if (!nodesById.TryGetValue(edge.FromNodeId, out var from) ||
                !nodesById.TryGetValue(edge.ToNodeId, out var to))
            {
                continue;
            }

            AddNeighbor(adjacency, edge.FromNodeId, edge.ToNodeId, edge.CostOverride ?? Distance(from, to));

            if (edge.IsBidirectional)
                AddNeighbor(adjacency, edge.ToNodeId, edge.FromNodeId, edge.CostOverride ?? Distance(to, from));
        }

        return adjacency;
    }

    private static void AddNeighbor(Dictionary<Guid, List<RouteNeighbor>> adjacency, Guid from, Guid to, double cost)
    {
        if (!adjacency.TryGetValue(from, out var neighbors))
        {
            neighbors = new List<RouteNeighbor>();
            adjacency[from] = neighbors;
        }

        neighbors.Add(new RouteNeighbor(to, cost));
    }

    private static IReadOnlyList<RouteNode> ReconstructPath(
        IReadOnlyDictionary<Guid, Guid> cameFrom,
        IReadOnlyDictionary<Guid, RouteNode> nodesById,
        Guid current)
    {
        var path = new List<RouteNode> { nodesById[current] };

        while (cameFrom.TryGetValue(current, out var previous))
        {
            current = previous;
            path.Add(nodesById[current]);
        }

        path.Reverse();
        return path;
    }

    private static double Heuristic(RouteNode from, RouteNode to) => Distance(from, to);

    private sealed record RouteNeighbor(Guid NodeId, double Cost);
}

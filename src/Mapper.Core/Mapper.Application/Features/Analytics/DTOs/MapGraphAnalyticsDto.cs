namespace Mapper.Application.Features.Analytics.DTOs;

public class MapGraphAnalyticsDto
{
    public int MapCount { get; set; }
    public int TransitionCount { get; set; }
    public int IsolatedMapCount { get; set; }
    public int WeakComponentCount { get; set; }
    public double DirectedReachabilityRatio { get; set; }
    public List<MapGraphNodeDto> Nodes { get; set; } = [];
    public List<MapGraphEdgeDto> Edges { get; set; } = [];
    public List<MapGraphComponentDto> WeakComponents { get; set; } = [];
    public List<MapGraphNodeDto> Bottlenecks { get; set; } = [];
    public List<MapShortestPathDto> ShortestPaths { get; set; } = [];
}

public class MapGraphNodeDto
{
    public Guid GeoMapId { get; set; }
    public string Name { get; set; } = default!;
    public int IncomingTransitions { get; set; }
    public int OutgoingTransitions { get; set; }
    public int ReachableMapCount { get; set; }
    public double ReachabilityRatio { get; set; }
    public bool IsIsolated { get; set; }
}

public class MapGraphEdgeDto
{
    public Guid TransitionMarkId { get; set; }
    public Guid SourceGeoMapId { get; set; }
    public string SourceMapName { get; set; } = default!;
    public Guid TargetGeoMapId { get; set; }
    public string? TargetMapName { get; set; }
    public string Title { get; set; } = default!;
    public bool TargetExists { get; set; }
}

public class MapGraphComponentDto
{
    public int Index { get; set; }
    public int MapCount { get; set; }
    public List<Guid> GeoMapIds { get; set; } = [];
    public List<string> MapNames { get; set; } = [];
}

public class MapShortestPathDto
{
    public Guid SourceGeoMapId { get; set; }
    public Guid TargetGeoMapId { get; set; }
    public bool Exists { get; set; }
    public int Distance { get; set; }
    public List<Guid> Path { get; set; } = [];
}

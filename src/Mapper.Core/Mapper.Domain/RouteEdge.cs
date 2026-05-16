namespace Mapper.Domain;

public sealed class RouteEdge
{
    public Guid Id { get; private init; } = Guid.NewGuid();
    public Guid GeoMapId { get; private set; }
    public Guid FromNodeId { get; private set; }
    public Guid ToNodeId { get; private set; }
    public double? CostOverride { get; private set; }
    public bool IsBidirectional { get; private set; }
    public bool IsDisabled { get; private set; }
    public string? Description { get; private set; }

    private RouteEdge() { } // EF

    public RouteEdge(
        Guid geoMapId,
        Guid fromNodeId,
        Guid toNodeId,
        bool isBidirectional = true,
        double? costOverride = null,
        string? description = null)
    {
        GeoMapId = geoMapId;
        FromNodeId = fromNodeId;
        ToNodeId = toNodeId;
        IsBidirectional = isBidirectional;
        CostOverride = costOverride;
        Description = description;
    }

    public void Update(bool isBidirectional, double? costOverride, bool isDisabled, string? description)
    {
        IsBidirectional = isBidirectional;
        CostOverride = costOverride;
        IsDisabled = isDisabled;
        Description = description;
    }
}

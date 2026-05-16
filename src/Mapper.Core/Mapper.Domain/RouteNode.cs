namespace Mapper.Domain;

public sealed class RouteNode
{
    public Guid Id { get; private init; } = Guid.NewGuid();
    public Guid GeoMapId { get; private set; }
    public Guid? GeoMarkId { get; private set; }
    public double X { get; private set; }
    public double Y { get; private set; }
    public string? Title { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    private RouteNode() { } // EF

    public RouteNode(Guid geoMapId, double x, double y, string? title = null, Guid? geoMarkId = null)
    {
        GeoMapId = geoMapId;
        X = x;
        Y = y;
        Title = title;
        GeoMarkId = geoMarkId;
    }

    public void Update(double x, double y, string? title, Guid? geoMarkId)
    {
        X = x;
        Y = y;
        Title = title;
        GeoMarkId = geoMarkId;
    }

    public void SoftDelete(DateTimeOffset? deletedAt = null)
    {
        if (IsDeleted) return;
        IsDeleted = true;
        DeletedAt = deletedAt ?? DateTimeOffset.UtcNow;
    }
}

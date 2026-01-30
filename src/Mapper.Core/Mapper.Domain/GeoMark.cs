namespace Mapper.Domain;

public enum GeoMarkType
{
    Transition = 1,
    Workplace = 2,
    Camera = 3
}

public abstract class GeoMark
{
    public Guid Id { get; private init; } = Guid.NewGuid();
    public Guid GeoMapId { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public GeoMarkType Type { get; private set; }
    public double X { get; private set; }
    public double Y { get; private set; }
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    protected GeoMark() { } // EF
    protected GeoMark(Guid geoMapId, GeoMarkType type, double x, double y, string title, string? description)
    {
        GeoMapId = geoMapId;
        Type = type;
        X = x;
        Y = y;
        Title = title;
        Description = description;
    }
    public void Move(double x, double y)
    {
        X = x;
        Y = y;
    }
    public void UpdateText(string title, string? description)
    {
        Title = title;
        Description = description;
    }
    public void SoftDelete(DateTimeOffset? deletedAt = null)
    {
        if (IsDeleted) return;
        IsDeleted = true;
        DeletedAt = deletedAt ?? DateTimeOffset.UtcNow;
    }
}

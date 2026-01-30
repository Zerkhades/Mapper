namespace Mapper.Domain;

public class GeoMap
{
    public Guid Id { get; private init; } = Guid.NewGuid();
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public string ImagePath { get; private set; } = default!;
    public int ImageWidth { get; private set; }
    public int ImageHeight { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public ICollection<GeoMark> Marks { get; private set; } = new List<GeoMark>();
    private GeoMap() { } // EF
    public GeoMap(string name, string imagePath, int width, int height, string? description = null)
    {
        Name = name;
        ImagePath = imagePath;
        ImageWidth = width;
        ImageHeight = height;
        Description = description;
    }
    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
    }
    public void SoftDelete(DateTimeOffset? deletedAt = null)
    {
        if (IsDeleted) return;
        IsDeleted = true;
        DeletedAt = deletedAt ?? DateTimeOffset.UtcNow;
    }
}

namespace Mapper.Domain;

public sealed class TransitionMark : GeoMark
{
    public Guid TargetGeoMapId { get; private set; }

    private TransitionMark() { } // EF

    public TransitionMark(Guid geoMapId, double x, double y, string title, Guid targetGeoMapId, string? description = null)
        : base(geoMapId, GeoMarkType.Transition, x, y, title, description)
    {
        TargetGeoMapId = targetGeoMapId;
    }

    public void SetTarget(Guid targetGeoMapId) => TargetGeoMapId = targetGeoMapId;
}

namespace Mapper.Domain;

public sealed class CameraMark : GeoMark
{
    public string? CameraName { get; private set; }
    public string? StreamUrl { get; private set; } // rtsp/http/webrtc etc.

    private CameraMark() { } // EF

    public CameraMark(Guid geoMapId, double x, double y, string title, string? cameraName, string? streamUrl, string? description = null)
        : base(geoMapId, GeoMarkType.Camera, x, y, title, description)
    {
        CameraName = cameraName;
        StreamUrl = streamUrl;
    }

    public void UpdateCamera(string? name, string? streamUrl)
    {
        CameraName = name;
        StreamUrl = streamUrl;
    }
}

using Mapper.Application.Interfaces;

namespace Mapper.Infrastructure.Cameras;

public class FakeCameraAdapter : ICameraAdapter
{
    // Минимальный валидный PNG 1x1 (прозрачный)
    private static readonly byte[] Png1x1 =
    {
        137,80,78,71,13,10,26,10,0,0,0,13,73,72,68,82,0,0,0,1,0,0,0,1,8,6,0,0,0,31,21,196,137,
        0,0,0,10,73,68,65,84,120,156,99,0,1,0,0,5,0,1,13,10,44,59,0,0,0,0,73,69,78,68,174,66,96,130
    };

    public Task<CameraStatus> GetStatusAsync(string? streamUrl, CancellationToken ct)
    {
        var seconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var online = (seconds / 30) % 2 == 0;

        var status = new CameraStatus(
            IsOnline: online,
            RttMs: online ? 12 : null,
            Message: online ? "Fake camera online" : "Fake camera offline"
        );

        return Task.FromResult(status);
    }

    public Task<CameraSnapshot?> TryGetSnapshotAsync(string? streamUrl, CancellationToken ct)
    {
        var snap = new CameraSnapshot(
            Bytes: Png1x1,
            ContentType: "image/png",
            FileName: "snapshot.png"
        );

        return Task.FromResult<CameraSnapshot?>(snap);
    }
}

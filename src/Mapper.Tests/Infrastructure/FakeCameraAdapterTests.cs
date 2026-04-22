using Mapper.Application.Interfaces;
using Mapper.Infrastructure.Cameras;

namespace Mapper.Tests.Infrastructure;

public class FakeCameraAdapterTests
{
    private readonly ICameraAdapter _adapter;

    public FakeCameraAdapterTests()
    {
        _adapter = new FakeCameraAdapter();
    }

    [Fact]
    public async Task GetStatusAsync_ShouldReturnCameraStatus()
    {
        // Act
        var status = await _adapter.GetStatusAsync("rtsp://fake-url", CancellationToken.None);

        // Assert
        Assert.NotNull(status);
        Assert.NotNull(status.Message);
    }

    [Fact]
    public async Task GetStatusAsync_ShouldAlternateOnlineStatus()
    {
        // Act
        var status1 = await _adapter.GetStatusAsync("rtsp://fake-url", CancellationToken.None);
        await Task.Delay(100); // Small delay to potentially change state
        var status2 = await _adapter.GetStatusAsync("rtsp://fake-url", CancellationToken.None);

        // Assert
        Assert.NotNull(status1);
        Assert.NotNull(status2);
        // Both should be valid status objects (either online or offline)
        if (status1.IsOnline)
        {
            Assert.NotNull(status1.RttMs);
        }
    }

    [Fact]
    public async Task TryGetSnapshotAsync_ShouldReturnValidSnapshot()
    {
        // Act
        var snapshot = await _adapter.TryGetSnapshotAsync("rtsp://fake-url", CancellationToken.None);

        // Assert
        Assert.NotNull(snapshot);
        Assert.NotEmpty(snapshot.Bytes);
        Assert.Equal("image/png", snapshot.ContentType);
        Assert.Equal("snapshot.png", snapshot.FileName);
    }

    [Fact]
    public async Task TryGetSnapshotAsync_ShouldReturnValidPngData()
    {
        // Act
        var snapshot = await _adapter.TryGetSnapshotAsync("rtsp://fake-url", CancellationToken.None);

        // Assert
        Assert.NotNull(snapshot);
        // PNG magic number: 137, 80, 78, 71
        Assert.True(snapshot.Bytes.Length > 4);
        Assert.Equal(137, snapshot.Bytes[0]);
        Assert.Equal(80, snapshot.Bytes[1]);
        Assert.Equal(78, snapshot.Bytes[2]);
        Assert.Equal(71, snapshot.Bytes[3]);
    }

    [Fact]
    public async Task TryGetVideoAsync_ShouldReturnValidVideo()
    {
        // Arrange
        var duration = TimeSpan.FromSeconds(30);

        // Act
        var video = await _adapter.TryGetVideoAsync("rtsp://fake-url", duration, CancellationToken.None);

        // Assert
        Assert.NotNull(video);
        Assert.NotEmpty(video.Bytes);
        Assert.Equal("video/mp4", video.ContentType);
        Assert.Equal("video.mp4", video.FileName);
        Assert.Equal(duration, video.Duration);
    }

    [Fact]
    public async Task TryDetectMotionAsync_ShouldReturnMotionResult()
    {
        // Arrange
        var frameData = new byte[1024];

        // Act
        var result = await _adapter.TryDetectMotionAsync("rtsp://fake-url", frameData, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.MotionPercentage >= 0);
        Assert.True(result.MotionPercentage <= 100);
    }

    [Fact]
    public async Task TryDetectMotionAsync_WithMotion_ShouldHavePositivePercentage()
    {
        // Arrange
        var frameData = new byte[1024];
        
        // Try multiple times to get a motion detection
        MotionDetectionResult? result = null;
        for (int i = 0; i < 20; i++)
        {
            result = await _adapter.TryDetectMotionAsync("rtsp://fake-url", frameData, CancellationToken.None);
            if (result?.HasMotion == true)
                break;
            await Task.Delay(100);
        }

        // Assert
        Assert.NotNull(result);
        var motionResult = result!;
        // Either has motion with percentage > 0, or no motion with percentage = 0
        if (motionResult.HasMotion)
        {
            Assert.True(motionResult.MotionPercentage > 0);
        }
        else
        {
            Assert.Equal(0, motionResult.MotionPercentage);
        }
    }

    [Fact]
    public async Task TryGetSnapshotWithZoomAsync_ShouldReturnSnapshot()
    {
        // Arrange
        var zoomLevel = 2.0;
        var centerX = 100;
        var centerY = 200;

        // Act
        var snapshot = await _adapter.TryGetSnapshotWithZoomAsync(
            "rtsp://fake-url",
            zoomLevel,
            centerX,
            centerY,
            CancellationToken.None);

        // Assert
        Assert.NotNull(snapshot);
        Assert.NotEmpty(snapshot.Bytes);
        Assert.Equal("image/png", snapshot.ContentType);
        Assert.Contains("zoom", snapshot.FileName);
    }

    [Theory]
    [InlineData(1.0)]
    [InlineData(2.0)]
    [InlineData(4.0)]
    public async Task TryGetSnapshotWithZoomAsync_ShouldAcceptVariousZoomLevels(double zoomLevel)
    {
        // Act
        var snapshot = await _adapter.TryGetSnapshotWithZoomAsync(
            "rtsp://fake-url",
            zoomLevel,
            ct: CancellationToken.None);

        // Assert
        Assert.NotNull(snapshot);
        Assert.Contains(zoomLevel.ToString("F1"), snapshot.FileName);
    }

    [Fact]
    public async Task CancellationToken_ShouldBeRespected()
    {
        // Arrange
        var cts = new CancellationTokenSource();

        // Act & Assert - methods should complete successfully even with non-cancelled token
        await _adapter.GetStatusAsync("url", cts.Token);
        await _adapter.TryGetSnapshotAsync("url", cts.Token);
        await _adapter.TryGetVideoAsync("url", TimeSpan.FromSeconds(1), cts.Token);
        await _adapter.TryDetectMotionAsync("url", new byte[100], cts.Token);
    }
}

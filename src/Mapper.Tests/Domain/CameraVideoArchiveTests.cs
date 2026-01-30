using Mapper.Domain;

namespace Mapper.Tests.Domain;

public class CameraVideoArchiveTests
{
    [Fact]
    public void Constructor_ShouldCreateVideoArchiveWithValidProperties()
    {
        // Arrange
        var cameraMarkId = Guid.NewGuid();
        var videoPath = "/videos/camera1_20260128.mp4";
        var duration = TimeSpan.FromMinutes(5);
        var fileSizeBytes = 10485760L; // 10 MB
        var resolution = "1920x1080";
        var fps = 30;
        var thumbnailPath = "/thumbnails/camera1_20260128.jpg";

        // Act
        var archive = new CameraVideoArchive(
            cameraMarkId,
            videoPath,
            duration,
            fileSizeBytes,
            resolution,
            fps,
            thumbnailPath);

        // Assert
        Assert.NotEqual(Guid.Empty, archive.Id);
        Assert.Equal(cameraMarkId, archive.CameraMarkId);
        Assert.Equal(videoPath, archive.VideoPath);
        Assert.Equal(duration, archive.Duration);
        Assert.Equal(fileSizeBytes, archive.FileSizeBytes);
        Assert.Equal(resolution, archive.Resolution);
        Assert.Equal(fps, archive.FramesPerSecond);
        Assert.Equal(thumbnailPath, archive.ThumbnailPath);
        Assert.False(archive.HasMotionDetected);
        Assert.False(archive.IsArchived);
    }

    [Fact]
    public void SetMotionDetected_ShouldUpdateMotionFlag()
    {
        // Arrange
        var archive = new CameraVideoArchive(
            Guid.NewGuid(),
            "/video.mp4",
            TimeSpan.FromMinutes(1),
            1000000,
            "1280x720",
            25);

        // Act
        archive.SetMotionDetected(true);

        // Assert
        Assert.True(archive.HasMotionDetected);
    }

    [Fact]
    public void Archive_ShouldMarkAsArchived()
    {
        // Arrange
        var archive = new CameraVideoArchive(
            Guid.NewGuid(),
            "/video.mp4",
            TimeSpan.FromMinutes(1),
            1000000,
            "1280x720",
            25);

        // Act
        archive.Archive();

        // Assert
        Assert.True(archive.IsArchived);
    }

    [Fact]
    public void RecordedAt_ShouldBeSetOnCreation()
    {
        // Arrange
        var beforeCreation = DateTimeOffset.UtcNow;

        // Act
        var archive = new CameraVideoArchive(
            Guid.NewGuid(),
            "/video.mp4",
            TimeSpan.FromMinutes(1),
            1000000,
            "1920x1080",
            30);

        // Assert
        Assert.True(archive.RecordedAt >= beforeCreation);
        Assert.True(archive.RecordedAt <= DateTimeOffset.UtcNow);
    }

    [Theory]
    [InlineData("640x480", 15)]
    [InlineData("1280x720", 25)]
    [InlineData("1920x1080", 30)]
    [InlineData("3840x2160", 60)]
    public void Constructor_ShouldAcceptVariousResolutionsAndFps(string resolution, int fps)
    {
        // Act
        var archive = new CameraVideoArchive(
            Guid.NewGuid(),
            "/video.mp4",
            TimeSpan.FromMinutes(1),
            1000000,
            resolution,
            fps);

        // Assert
        Assert.Equal(resolution, archive.Resolution);
        Assert.Equal(fps, archive.FramesPerSecond);
    }
}

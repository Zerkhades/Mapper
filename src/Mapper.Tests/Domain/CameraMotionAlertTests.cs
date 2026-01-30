using Mapper.Domain;

namespace Mapper.Tests.Domain;

public class CameraMotionAlertTests
{
    [Fact]
    public void Constructor_ShouldCreateAlertWithValidProperties()
    {
        // Arrange
        var cameraMarkId = Guid.NewGuid();
        var severity = MotionSeverity.High;
        var motionPercentage = 75.5;
        var snapshotPath = "/snapshots/alert123.jpg";

        // Act
        var alert = new CameraMotionAlert(cameraMarkId, severity, motionPercentage, snapshotPath);

        // Assert
        Assert.NotEqual(Guid.Empty, alert.Id);
        Assert.Equal(cameraMarkId, alert.CameraMarkId);
        Assert.Equal(severity, alert.Severity);
        Assert.Equal(motionPercentage, alert.MotionPercentage);
        Assert.Equal(snapshotPath, alert.SnapshotPath);
        Assert.False(alert.IsResolved);
        Assert.Null(alert.ConfirmedAt);
        Assert.Null(alert.RelatedVideoArchiveId);
    }

    [Fact]
    public void Confirm_ShouldSetConfirmedAt()
    {
        // Arrange
        var alert = new CameraMotionAlert(Guid.NewGuid(), MotionSeverity.Medium, 50.0);
        var beforeConfirm = DateTimeOffset.UtcNow;

        // Act
        alert.Confirm();

        // Assert
        Assert.NotNull(alert.ConfirmedAt);
        Assert.True(alert.ConfirmedAt >= beforeConfirm);
    }

    [Fact]
    public void Resolve_ShouldMarkAsResolved()
    {
        // Arrange
        var alert = new CameraMotionAlert(Guid.NewGuid(), MotionSeverity.Low, 25.0);
        var notes = "False alarm - cleaning staff";

        // Act
        alert.Resolve(notes);

        // Assert
        Assert.True(alert.IsResolved);
        Assert.Equal(notes, alert.ResolutionNotes);
    }

    [Fact]
    public void LinkToVideo_ShouldSetRelatedVideoArchiveId()
    {
        // Arrange
        var alert = new CameraMotionAlert(Guid.NewGuid(), MotionSeverity.High, 80.0);
        var videoId = Guid.NewGuid();

        // Act
        alert.LinkToVideo(videoId);

        // Assert
        Assert.Equal(videoId, alert.RelatedVideoArchiveId);
    }

    [Theory]
    [InlineData(MotionSeverity.Low)]
    [InlineData(MotionSeverity.Medium)]
    [InlineData(MotionSeverity.High)]
    public void Constructor_ShouldAcceptAllSeverityLevels(MotionSeverity severity)
    {
        // Act
        var alert = new CameraMotionAlert(Guid.NewGuid(), severity, 50.0);

        // Assert
        Assert.Equal(severity, alert.Severity);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(50.0)]
    [InlineData(100.0)]
    public void Constructor_ShouldAcceptValidMotionPercentages(double percentage)
    {
        // Act
        var alert = new CameraMotionAlert(Guid.NewGuid(), MotionSeverity.Medium, percentage);

        // Assert
        Assert.Equal(percentage, alert.MotionPercentage);
    }
}

using Mapper.Application.Features.DTOs;
using Mapper.Domain;

namespace Mapper.Tests.DTOs;

public class CameraDtoTests
{
    [Fact]
    public void CameraVideoArchiveDto_ShouldHaveAllProperties()
    {
        // Arrange & Act
        var dto = new CameraVideoArchiveDto
        {
            Id = Guid.NewGuid(),
            CameraMarkId = Guid.NewGuid(),
            VideoPath = "/videos/test.mp4",
            ThumbnailPath = "/thumbnails/test.jpg",
            RecordedAt = DateTimeOffset.UtcNow,
            Duration = TimeSpan.FromMinutes(5),
            FileSizeBytes = 10485760,
            HasMotionDetected = true,
            Resolution = "1920x1080",
            FramesPerSecond = 30,
            IsArchived = false,
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Assert
        Assert.NotEqual(Guid.Empty, dto.Id);
        Assert.NotEqual(Guid.Empty, dto.CameraMarkId);
        Assert.NotNull(dto.VideoPath);
        Assert.NotNull(dto.ThumbnailPath);
        Assert.True(dto.Duration.TotalMinutes > 0);
        Assert.True(dto.FileSizeBytes > 0);
        Assert.NotNull(dto.Resolution);
        Assert.True(dto.FramesPerSecond > 0);
    }

    [Fact]
    public void CameraMotionAlertDto_ShouldHaveAllProperties()
    {
        // Arrange & Act
        var dto = new CameraMotionAlertDto
        {
            Id = Guid.NewGuid(),
            CameraMarkId = Guid.NewGuid(),
            DetectedAt = DateTimeOffset.UtcNow,
            ConfirmedAt = DateTimeOffset.UtcNow,
            Severity = "High",
            MotionPercentage = 85.5,
            SnapshotUrl = "/snapshots/alert.jpg",
            IsResolved = false,
            ResolutionNotes = null,
            RelatedVideoArchiveId = Guid.NewGuid()
        };

        // Assert
        Assert.NotEqual(Guid.Empty, dto.Id);
        Assert.NotEqual(Guid.Empty, dto.CameraMarkId);
        Assert.NotNull(dto.Severity);
        Assert.True(dto.MotionPercentage >= 0 && dto.MotionPercentage <= 100);
        Assert.NotNull(dto.SnapshotUrl);
        Assert.NotNull(dto.RelatedVideoArchiveId);
    }

    [Fact]
    public void CameraStatusHistoryDto_ShouldHaveAllProperties()
    {
        // Arrange & Act
        var dto = new CameraStatusHistoryDto
        {
            Id = Guid.NewGuid(),
            CameraMarkId = Guid.NewGuid(),
            IsOnline = true,
            Reason = "NetworkConnected",
            ChangedAt = DateTimeOffset.UtcNow,
            DurationSinceLastChange = TimeSpan.FromMinutes(30),
            Details = "Camera connected successfully",
            ResponseTimeMs = 150
        };

        // Assert
        Assert.NotEqual(Guid.Empty, dto.Id);
        Assert.NotEqual(Guid.Empty, dto.CameraMarkId);
        Assert.True(dto.IsOnline);
        Assert.NotNull(dto.Reason);
        Assert.NotNull(dto.Details);
        Assert.NotNull(dto.ResponseTimeMs);
        Assert.True(dto.ResponseTimeMs > 0);
    }

    [Fact]
    public void MotionSeverity_ShouldHaveAllValues()
    {
        // Arrange & Act
        var lowSeverity = MotionSeverity.Low;
        var mediumSeverity = MotionSeverity.Medium;
        var highSeverity = MotionSeverity.High;

        // Assert
        Assert.Equal(1, (int)lowSeverity);
        Assert.Equal(2, (int)mediumSeverity);
        Assert.Equal(3, (int)highSeverity);
    }

    [Fact]
    public void CameraStatusReason_ShouldHaveAllValues()
    {
        // Act
        var reasons = Enum.GetValues<CameraStatusReason>();

        // Assert
        Assert.Contains(CameraStatusReason.Unknown, reasons);
        Assert.Contains(CameraStatusReason.NetworkConnected, reasons);
        Assert.Contains(CameraStatusReason.NetworkDisconnected, reasons);
        Assert.Contains(CameraStatusReason.PowerOn, reasons);
        Assert.Contains(CameraStatusReason.PowerOff, reasons);
        Assert.Contains(CameraStatusReason.NetworkTimeout, reasons);
        Assert.Contains(CameraStatusReason.HardwareError, reasons);
    }
}

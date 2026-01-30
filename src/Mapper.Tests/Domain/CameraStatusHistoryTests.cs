using Mapper.Domain;

namespace Mapper.Tests.Domain;

public class CameraStatusHistoryTests
{
    [Fact]
    public void Constructor_ShouldCreateStatusHistoryWithValidProperties()
    {
        // Arrange
        var cameraMarkId = Guid.NewGuid();
        var isOnline = true;
        var reason = CameraStatusReason.NetworkConnected;
        var details = "Camera successfully connected to network";
        var responseTime = 150;

        // Act
        var statusHistory = new CameraStatusHistory(
            cameraMarkId,
            isOnline,
            reason,
            details,
            responseTime
        );

        // Assert
        Assert.NotEqual(Guid.Empty, statusHistory.Id);
        Assert.Equal(cameraMarkId, statusHistory.CameraMarkId);
        Assert.Equal(isOnline, statusHistory.IsOnline);
        Assert.Equal(reason, statusHistory.Reason);
        Assert.Equal(details, statusHistory.Details);
        Assert.Equal(responseTime, statusHistory.ResponseTimeMs);
        Assert.True(statusHistory.ChangedAt <= DateTimeOffset.UtcNow);
    }

    [Theory]
    [InlineData(true, CameraStatusReason.NetworkConnected)]
    [InlineData(true, CameraStatusReason.PowerOn)]
    [InlineData(false, CameraStatusReason.NetworkDisconnected)]
    [InlineData(false, CameraStatusReason.PowerOff)]
    [InlineData(false, CameraStatusReason.NetworkTimeout)]
    public void Constructor_ShouldAcceptVariousStatusesAndReasons(bool isOnline, CameraStatusReason reason)
    {
        // Act
        var statusHistory = new CameraStatusHistory(
            Guid.NewGuid(),
            isOnline,
            reason
        );

        // Assert
        Assert.Equal(isOnline, statusHistory.IsOnline);
        Assert.Equal(reason, statusHistory.Reason);
    }

    [Fact]
    public void Constructor_WithoutOptionalParameters_ShouldCreateValidObject()
    {
        // Act
        var statusHistory = new CameraStatusHistory(
            Guid.NewGuid(),
            true,
            CameraStatusReason.NetworkConnected
        );

        // Assert
        Assert.Null(statusHistory.Details);
        Assert.Null(statusHistory.ResponseTimeMs);
    }

    [Fact]
    public void ChangedAt_ShouldBeSetOnCreation()
    {
        // Arrange
        var beforeCreation = DateTimeOffset.UtcNow;

        // Act
        var statusHistory = new CameraStatusHistory(
            Guid.NewGuid(),
            true,
            CameraStatusReason.PowerOn
        );

        // Assert
        Assert.True(statusHistory.ChangedAt >= beforeCreation);
        Assert.True(statusHistory.ChangedAt <= DateTimeOffset.UtcNow);
    }

    [Theory]
    [InlineData(CameraStatusReason.Unknown)]
    [InlineData(CameraStatusReason.NetworkConnected)]
    [InlineData(CameraStatusReason.NetworkDisconnected)]
    [InlineData(CameraStatusReason.Reboot)]
    [InlineData(CameraStatusReason.PowerOn)]
    [InlineData(CameraStatusReason.PowerOff)]
    [InlineData(CameraStatusReason.ManualReset)]
    [InlineData(CameraStatusReason.ConfigurationChanged)]
    [InlineData(CameraStatusReason.FirmwareUpdate)]
    [InlineData(CameraStatusReason.NetworkTimeout)]
    [InlineData(CameraStatusReason.DNSResolutionFailed)]
    [InlineData(CameraStatusReason.Unauthorized)]
    [InlineData(CameraStatusReason.CertificateError)]
    [InlineData(CameraStatusReason.HardwareError)]
    [InlineData(CameraStatusReason.OutOfMemory)]
    [InlineData(CameraStatusReason.ThermalShutdown)]
    public void Constructor_ShouldAcceptAllReasonTypes(CameraStatusReason reason)
    {
        // Act
        var statusHistory = new CameraStatusHistory(
            Guid.NewGuid(),
            true,
            reason
        );

        // Assert
        Assert.Equal(reason, statusHistory.Reason);
    }

    [Fact]
    public void Constructor_WithResponseTime_ShouldStoreValue()
    {
        // Arrange
        var responseTime = 250;

        // Act
        var statusHistory = new CameraStatusHistory(
            Guid.NewGuid(),
            true,
            CameraStatusReason.NetworkConnected,
            responseTimeMs: responseTime
        );

        // Assert
        Assert.Equal(responseTime, statusHistory.ResponseTimeMs);
    }
}

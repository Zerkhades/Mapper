using Mapper.Application.Features.Notifications.Queries;
using Mapper.Domain;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using System.Reflection;

namespace Mapper.Tests.Notifications;

public class GetSmartNotificationsQueryHandlerTests : TestCommandBase
{
    private readonly CameraArchiveContextFactory _factory;

    public GetSmartNotificationsQueryHandlerTests() : base(new CameraArchiveContextFactory())
    {
        _factory = (CameraArchiveContextFactory)ContextFactory;
    }

    [Fact]
    public async Task Handle_WithOfflineCameraBeyondGracePeriod_ShouldReturnOfflineNotification()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        AddStatus(_factory.CameraMarkId, false, CameraStatusReason.NetworkDisconnected, now.AddMinutes(-10));
        await Context.SaveChangesAsync();

        var handler = new GetSmartNotificationsQueryHandler(Context);

        // Act
        var notifications = await handler.Handle(
            new GetSmartNotificationsQuery(now.AddHours(-1), now, OfflineGraceMinutes: 5),
            CancellationToken.None);

        // Assert
        var notification = Assert.Single(notifications);
        Assert.Equal("CameraOffline", notification.Type);
        Assert.Equal("Warning", notification.Severity);
        Assert.Equal(_factory.CameraMarkId, notification.CameraMarkId);
        Assert.Equal("10", notification.Context["offlineMinutes"]);
    }

    [Fact]
    public async Task Handle_WithRecentOfflineCameraInsideGracePeriod_ShouldSuppressOfflineNotification()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        AddStatus(_factory.CameraMarkId, false, CameraStatusReason.NetworkDisconnected, now.AddMinutes(-2));
        await Context.SaveChangesAsync();

        var handler = new GetSmartNotificationsQueryHandler(Context);

        // Act
        var notifications = await handler.Handle(
            new GetSmartNotificationsQuery(now.AddHours(-1), now, OfflineGraceMinutes: 5),
            CancellationToken.None);

        // Assert
        Assert.Empty(notifications);
    }

    [Fact]
    public async Task Handle_WithUnresolvedHighMotionAlert_ShouldReturnCriticalNotification()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var alert = AddAlert(_factory.CameraMarkId, MotionSeverity.High, 85, false, now.AddMinutes(-3));
        await Context.SaveChangesAsync();

        var handler = new GetSmartNotificationsQueryHandler(Context);

        // Act
        var notifications = await handler.Handle(
            new GetSmartNotificationsQuery(now.AddHours(-1), now),
            CancellationToken.None);

        // Assert
        var notification = Assert.Single(notifications);
        Assert.Equal("HighMotion", notification.Type);
        Assert.Equal("Critical", notification.Severity);
        Assert.Equal(alert.Id, notification.RelatedEntityId);
        Assert.Equal("85.00", notification.Context["motionPercentage"]);
    }

    [Fact]
    public async Task Handle_WithMotionSpike_ShouldReturnSpikeNotification()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        AddAlert(_factory.CameraMarkId, MotionSeverity.Low, 10, true, now.AddHours(-25));

        for (var i = 1; i <= 6; i++)
        {
            AddAlert(_factory.CameraMarkId, MotionSeverity.Low, 20 + i, true, now.AddMinutes(-i));
        }

        await Context.SaveChangesAsync();
        var handler = new GetSmartNotificationsQueryHandler(Context);

        // Act
        var notifications = await handler.Handle(
            new GetSmartNotificationsQuery(now.AddHours(-24), now),
            CancellationToken.None);

        // Assert
        var notification = Assert.Single(notifications);
        Assert.Equal("MotionSpike", notification.Type);
        Assert.Equal("Warning", notification.Severity);
        Assert.Equal("6", notification.Context["currentAlertCount"]);
    }

    private CameraMotionAlert AddAlert(
        Guid cameraMarkId,
        MotionSeverity severity,
        double motionPercentage,
        bool resolved,
        DateTimeOffset detectedAt)
    {
        var alert = new CameraMotionAlert(cameraMarkId, severity, motionPercentage);
        SetProperty(alert, "DetectedAt", detectedAt);
        if (resolved)
        {
            alert.Resolve();
        }

        Context.CameraMotionAlerts.Add(alert);
        return alert;
    }

    private void AddStatus(
        Guid cameraMarkId,
        bool isOnline,
        CameraStatusReason reason,
        DateTimeOffset changedAt)
    {
        var status = new CameraStatusHistory(cameraMarkId, isOnline, reason);
        SetProperty(status, "ChangedAt", changedAt);
        Context.CameraStatusHistories.Add(status);
    }

    private static void SetProperty<TValue>(object target, string propertyName, TValue value)
    {
        var property = target.GetType().GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        Assert.NotNull(property);
        property.SetValue(target, value);
    }
}

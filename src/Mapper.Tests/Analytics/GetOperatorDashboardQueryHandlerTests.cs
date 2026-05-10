using Mapper.Application.Features.Analytics.Queries;
using Mapper.Domain;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Mapper.Tests.Analytics;

public class GetOperatorDashboardQueryHandlerTests : TestCommandBase
{
    private readonly CameraArchiveContextFactory _factory;

    public GetOperatorDashboardQueryHandlerTests() : base(new CameraArchiveContextFactory())
    {
        _factory = (CameraArchiveContextFactory)ContextFactory;
    }

    [Fact]
    public async Task Handle_WithCameraActivity_ShouldReturnOperatorSummary()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var secondCamera = new CameraMark(
            _factory.GeoMapId,
            250,
            300,
            "Warehouse Camera",
            "CAM-002",
            "rtsp://192.168.1.101/stream");

        Context.GeoMarks.Add(secondCamera);
        AddStatus(_factory.CameraMarkId, true, CameraStatusReason.NetworkConnected, now.AddMinutes(-10));
        AddStatus(secondCamera.Id, false, CameraStatusReason.NetworkDisconnected, now.AddMinutes(-20));
        AddAlert(_factory.CameraMarkId, MotionSeverity.High, 80, false, now.AddHours(-1));
        AddAlert(_factory.CameraMarkId, MotionSeverity.Medium, 45, true, now.AddHours(-2));
        AddAlert(_factory.CameraMarkId, MotionSeverity.Low, 20, false, now.AddHours(-3));
        AddAlert(_factory.CameraMarkId, MotionSeverity.Low, 10, true, now.AddHours(-26));
        AddVideo(_factory.CameraMarkId, now.AddHours(-4));
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new GetOperatorDashboardQueryHandler(Context);

        // Act
        var dashboard = await handler.Handle(
            new GetOperatorDashboardQuery(now.AddHours(-24), now, 5),
            CancellationToken.None);

        // Assert
        Assert.Equal(2, dashboard.TotalCameras);
        Assert.Equal(1, dashboard.OnlineCameras);
        Assert.Equal(1, dashboard.OfflineCameras);
        Assert.Equal(3, dashboard.MotionAlerts);
        Assert.Equal(2, dashboard.UnresolvedMotionAlerts);
        Assert.Equal(1, dashboard.HighSeverityUnresolvedAlerts);
        Assert.Equal(1, dashboard.ArchivedVideos);
        Assert.Single(dashboard.ProblemCameras);
        Assert.Equal(secondCamera.Id, dashboard.ProblemCameras[0].CameraMarkId);
        Assert.Single(dashboard.TopActiveCameras);
        Assert.Equal(_factory.CameraMarkId, dashboard.TopActiveCameras[0].CameraMarkId);
    }

    [Fact]
    public async Task Handle_WithMotionSpike_ShouldReturnAnomaly()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        AddAlert(_factory.CameraMarkId, MotionSeverity.Low, 10, true, now.AddHours(-25));

        for (var i = 1; i <= 6; i++)
        {
            AddAlert(_factory.CameraMarkId, MotionSeverity.High, 70 + i, false, now.AddHours(-i));
        }

        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var handler = new GetOperatorDashboardQueryHandler(Context);

        // Act
        var dashboard = await handler.Handle(
            new GetOperatorDashboardQuery(now.AddHours(-24), now, 5),
            CancellationToken.None);

        // Assert
        Assert.Single(dashboard.Anomalies);
        Assert.Equal(_factory.CameraMarkId, dashboard.Anomalies[0].CameraMarkId);
        Assert.Equal(6, dashboard.Anomalies[0].CurrentAlertCount);
    }

    private void AddAlert(
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

    private void AddVideo(Guid cameraMarkId, DateTimeOffset recordedAt)
    {
        var video = new CameraVideoArchive(
            cameraMarkId,
            "/videos/test.mp4",
            TimeSpan.FromMinutes(1),
            1024,
            "1920x1080",
            30);
        SetProperty(video, "RecordedAt", recordedAt);
        Context.CameraVideoArchives.Add(video);
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

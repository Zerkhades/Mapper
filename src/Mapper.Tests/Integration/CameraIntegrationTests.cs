using Mapper.Application.Interfaces;
using Mapper.Domain;
using Mapper.Infrastructure.Cameras;
using Mapper.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Mapper.Tests.Integration;

public class CameraIntegrationTests
{
    private static void SetIdProperty(object obj, Guid id)
    {
        var backingField = obj.GetType().GetField("<Id>k__BackingField", 
            BindingFlags.Instance | BindingFlags.NonPublic);
        backingField?.SetValue(obj, id);
    }

    [Fact]
    public async Task FakeCameraAdapter_WithDatabase_ShouldWorkCorrectly()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MapperDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new MapperDbContext(options);

        var geoMapId = Guid.NewGuid();
        var geoMap = new GeoMap("Test Map", "/path.png", 1000, 1000);
        SetIdProperty(geoMap, geoMapId);

        var cameraMarkId = Guid.NewGuid();
        var cameraMark = new CameraMark(geoMapId, 100, 200, "Camera 1", "Test Camera", "rtsp://test");
        SetIdProperty(cameraMark, cameraMarkId);

        context.GeoMaps.Add(geoMap);
        context.GeoMarks.Add(cameraMark);
        await context.SaveChangesAsync();

        var adapter = new FakeCameraAdapter();

        // Act
        var status = await adapter.GetStatusAsync("rtsp://test", CancellationToken.None);
        var snapshot = await adapter.TryGetSnapshotAsync("rtsp://test", CancellationToken.None);

        // Assert
        Assert.NotNull(status);
        Assert.NotNull(snapshot);

        // Create camera status history
        var statusHistory = new CameraStatusHistory(
            cameraMarkId,
            status.IsOnline,
            status.IsOnline ? CameraStatusReason.NetworkConnected : CameraStatusReason.NetworkDisconnected,
            status.Message,
            status.RttMs
        );

        context.CameraStatusHistories.Add(statusHistory);
        await context.SaveChangesAsync();

        // Verify it was saved
        var savedHistory = await context.CameraStatusHistories
            .FirstOrDefaultAsync(x => x.CameraMarkId == cameraMarkId);

        Assert.NotNull(savedHistory);
        Assert.Equal(cameraMarkId, savedHistory.CameraMarkId);
    }

    [Fact]
    public async Task CompleteWorkflow_CreateCameraAndRecordVideo_ShouldWork()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MapperDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new MapperDbContext(options);

        // Create GeoMap - use auto-generated Id
        var geoMap = new GeoMap("Floor 1", "/maps/floor1.png", 2000, 1500);
        context.GeoMaps.Add(geoMap);
        await context.SaveChangesAsync();
        var geoMapId = geoMap.Id;

        // Create CameraMark - use auto-generated Id
        var cameraMark = new CameraMark(
            geoMapId,
            500,
            750,
            "Main Entrance Camera",
            "CAM-001",
            "rtsp://192.168.1.100/stream"
        );
        context.GeoMarks.Add(cameraMark);
        await context.SaveChangesAsync();
        var cameraMarkId = cameraMark.Id;

        var adapter = new FakeCameraAdapter();

        // Act - Get video from camera
        var video = await adapter.TryGetVideoAsync(
            "rtsp://192.168.1.100/stream",
            TimeSpan.FromMinutes(5),
            CancellationToken.None
        );

        Assert.NotNull(video);

        // Create video archive entry
        var archive = new CameraVideoArchive(
            cameraMarkId,
            $"/videos/{cameraMarkId}_{DateTimeOffset.UtcNow:yyyyMMddHHmmss}.mp4",
            video.Duration,
            video.Bytes.Length,
            "1920x1080",
            30
        );

        context.CameraVideoArchives.Add(archive);
        await context.SaveChangesAsync();

        // Detect motion
        var motionResult = await adapter.TryDetectMotionAsync(
            "rtsp://192.168.1.100/stream",
            new byte[1024],
            CancellationToken.None
        );

        Assert.NotNull(motionResult);

        if (motionResult.HasMotion)
        {
            var alert = new CameraMotionAlert(
                cameraMarkId,
                MotionSeverity.Medium,
                motionResult.MotionPercentage
            );

            context.CameraMotionAlerts.Add(alert);
            await context.SaveChangesAsync();

            // Link alert to video
            alert.LinkToVideo(archive.Id);
            await context.SaveChangesAsync();

            var linkedAlert = await context.CameraMotionAlerts
                .FirstOrDefaultAsync(x => x.RelatedVideoArchiveId == archive.Id);

            Assert.NotNull(linkedAlert);
        }

        // Verify all entities
        var savedCamera = await context.GeoMarks
            .OfType<CameraMark>()
            .FirstOrDefaultAsync(x => x.Id == cameraMarkId);

        var savedArchive = await context.CameraVideoArchives
            .FirstOrDefaultAsync(x => x.CameraMarkId == cameraMarkId);

        Assert.NotNull(savedCamera);
        Assert.NotNull(savedArchive);
        Assert.Equal(cameraMarkId, savedArchive.CameraMarkId);
    }

    [Fact]
    public async Task CameraStatusTracking_MultipleChanges_ShouldRecordHistory()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MapperDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new MapperDbContext(options);

        var geoMapId = Guid.NewGuid();
        var geoMap = new GeoMap("Test Map", "/path.png", 1000, 1000);
        SetIdProperty(geoMap, geoMapId);

        var cameraMarkId = Guid.NewGuid();
        var cameraMark = new CameraMark(geoMapId, 100, 200, "Camera 1", "Test", "rtsp://test");
        SetIdProperty(cameraMark, cameraMarkId);

        context.GeoMaps.Add(geoMap);
        context.GeoMarks.Add(cameraMark);
        await context.SaveChangesAsync();

        // Act - Simulate status changes
        var statuses = new[]
        {
            (true, CameraStatusReason.PowerOn),
            (true, CameraStatusReason.NetworkConnected),
            (false, CameraStatusReason.NetworkTimeout),
            (false, CameraStatusReason.NetworkDisconnected),
            (true, CameraStatusReason.NetworkConnected)
        };

        foreach (var (isOnline, reason) in statuses)
        {
            var statusHistory = new CameraStatusHistory(
                cameraMarkId,
                isOnline,
                reason
            );

            context.CameraStatusHistories.Add(statusHistory);
            await Task.Delay(10); // Small delay to ensure different timestamps
        }

        await context.SaveChangesAsync();

        // Assert
        var histories = await context.CameraStatusHistories
            .Where(x => x.CameraMarkId == cameraMarkId)
            .OrderBy(x => x.ChangedAt)
            .ToListAsync();

        Assert.Equal(5, histories.Count);
        Assert.True(histories[0].IsOnline); // PowerOn
        Assert.True(histories[1].IsOnline); // NetworkConnected
        Assert.False(histories[2].IsOnline); // NetworkTimeout
        Assert.False(histories[3].IsOnline); // NetworkDisconnected
        Assert.True(histories[4].IsOnline); // NetworkConnected again
    }
}

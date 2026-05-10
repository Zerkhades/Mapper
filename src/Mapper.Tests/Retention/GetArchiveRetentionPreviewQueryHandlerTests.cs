using Mapper.Application.Features.Retention.Queries;
using Mapper.Domain;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using System.Reflection;

namespace Mapper.Tests.Retention;

public class GetArchiveRetentionPreviewQueryHandlerTests : TestCommandBase
{
    private readonly CameraArchiveContextFactory _factory;

    public GetArchiveRetentionPreviewQueryHandlerTests() : base(new CameraArchiveContextFactory())
    {
        _factory = (CameraArchiveContextFactory)ContextFactory;
    }

    [Fact]
    public async Task Handle_WithExpiredVideos_ShouldReturnPreviewCandidates()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        AddVideo("/videos/no-motion-old.mp4", now.AddDays(-8), 100, hasMotion: false, isArchived: false);
        AddVideo("/videos/motion-old.mp4", now.AddDays(-91), 200, hasMotion: true, isArchived: false);
        AddVideo("/videos/archived-old.mp4", now.AddDays(-366), 300, hasMotion: true, isArchived: true);
        AddVideo("/videos/recent.mp4", now.AddDays(-2), 400, hasMotion: false, isArchived: false);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new GetArchiveRetentionPreviewQueryHandler(Context);

        // Act
        var preview = await handler.Handle(new GetArchiveRetentionPreviewQuery(Now: now), CancellationToken.None);

        // Assert
        Assert.Equal(3, preview.CandidateCount);
        Assert.Equal(600, preview.ReclaimableBytes);
        Assert.Contains(preview.Candidates, x => x.VideoPath == "/videos/no-motion-old.mp4" && x.RetentionDays == 7);
        Assert.Contains(preview.Candidates, x => x.VideoPath == "/videos/motion-old.mp4" && x.RetentionDays == 90);
        Assert.Contains(preview.Candidates, x => x.VideoPath == "/videos/archived-old.mp4" && x.RetentionDays == 365);
        Assert.DoesNotContain(preview.Candidates, x => x.VideoPath == "/videos/recent.mp4");
    }

    [Fact]
    public async Task Handle_WithCameraFilter_ShouldReturnOnlyCameraCandidates()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var otherCamera = new CameraMark(
            _factory.GeoMapId,
            0.2,
            0.3,
            "Other Camera",
            "CAM-002",
            "rtsp://camera/2");
        Context.GeoMarks.Add(otherCamera);
        AddVideo("/videos/target.mp4", now.AddDays(-8), 100, hasMotion: false, isArchived: false, _factory.CameraMarkId);
        AddVideo("/videos/other.mp4", now.AddDays(-8), 200, hasMotion: false, isArchived: false, otherCamera.Id);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new GetArchiveRetentionPreviewQueryHandler(Context);

        // Act
        var preview = await handler.Handle(
            new GetArchiveRetentionPreviewQuery(CameraMarkId: _factory.CameraMarkId, Now: now),
            CancellationToken.None);

        // Assert
        var candidate = Assert.Single(preview.Candidates);
        Assert.Equal("/videos/target.mp4", candidate.VideoPath);
        Assert.Equal(100, preview.ReclaimableBytes);
    }

    private void AddVideo(
        string path,
        DateTimeOffset recordedAt,
        long fileSizeBytes,
        bool hasMotion,
        bool isArchived,
        Guid? cameraMarkId = null)
    {
        var video = new CameraVideoArchive(
            cameraMarkId ?? _factory.CameraMarkId,
            path,
            TimeSpan.FromMinutes(1),
            fileSizeBytes,
            "1920x1080",
            30);

        if (hasMotion)
        {
            video.SetMotionDetected(true);
        }

        if (isArchived)
        {
            video.Archive();
        }

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

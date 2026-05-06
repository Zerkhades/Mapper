using Mapper.Application.Features.Retention.Commands;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Mapper.Tests.Retention;

public class CleanupArchiveRetentionCommandHandlerTests : TestCommandBase
{
    private readonly CameraArchiveContextFactory _factory;
    private readonly RecordingS3ObjectStorage _storage = new();

    public CleanupArchiveRetentionCommandHandlerTests() : base(new CameraArchiveContextFactory())
    {
        _factory = (CameraArchiveContextFactory)ContextFactory;
    }

    [Fact]
    public async Task Handle_WithDryRun_ShouldReturnCandidatesWithoutDeleting()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        AddVideo("/videos/old.mp4", "/thumbs/old.jpg", now.AddDays(-8), 100, hasMotion: false);
        await Context.SaveChangesAsync();

        var handler = new CleanupArchiveRetentionCommandHandler(Context, _storage);

        // Act
        var result = await handler.Handle(
            new CleanupArchiveRetentionCommand(Now: now, DryRun: true),
            CancellationToken.None);

        // Assert
        Assert.True(result.DryRun);
        Assert.Equal(1, result.CandidateCount);
        Assert.Equal(0, result.DeletedCount);
        Assert.Empty(_storage.DeletedKeys);
        Assert.Equal(1, await Context.CameraVideoArchives.CountAsync());
    }

    [Fact]
    public async Task Handle_WithRealCleanupWithoutConfirm_ShouldThrow()
    {
        // Arrange
        var handler = new CleanupArchiveRetentionCommandHandler(Context, _storage);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new CleanupArchiveRetentionCommand(DryRun: false, Confirm: false), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithConfirmedRealCleanup_ShouldDeleteStorageObjectsAndRows()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        AddVideo("/videos/old.mp4", "/thumbs/old.jpg", now.AddDays(-8), 100, hasMotion: false);
        AddVideo("/videos/recent.mp4", null, now.AddDays(-1), 200, hasMotion: false);
        await Context.SaveChangesAsync();

        var handler = new CleanupArchiveRetentionCommandHandler(Context, _storage);

        // Act
        var result = await handler.Handle(
            new CleanupArchiveRetentionCommand(Now: now, DryRun: false, Confirm: true),
            CancellationToken.None);

        // Assert
        Assert.False(result.DryRun);
        Assert.True(result.Confirmed);
        Assert.Equal(1, result.CandidateCount);
        Assert.Equal(1, result.DeletedCount);
        Assert.Contains("/videos/old.mp4", _storage.DeletedKeys);
        Assert.Contains("/thumbs/old.jpg", _storage.DeletedKeys);
        var remaining = await Context.CameraVideoArchives.SingleAsync();
        Assert.Equal("/videos/recent.mp4", remaining.VideoPath);
    }

    private void AddVideo(
        string path,
        string? thumbnailPath,
        DateTimeOffset recordedAt,
        long fileSizeBytes,
        bool hasMotion)
    {
        var video = new CameraVideoArchive(
            _factory.CameraMarkId,
            path,
            TimeSpan.FromMinutes(1),
            fileSizeBytes,
            "1920x1080",
            30,
            thumbnailPath);

        if (hasMotion)
        {
            video.SetMotionDetected(true);
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

    private sealed class RecordingS3ObjectStorage : IS3ObjectStorage
    {
        public List<string> DeletedKeys { get; } = [];

        public Task<string> PutAsync(string key, Stream content, string contentType, CancellationToken ct)
        {
            return Task.FromResult(key);
        }

        public Task DeleteAsync(string key, CancellationToken ct)
        {
            DeletedKeys.Add(key);
            return Task.CompletedTask;
        }
    }
}

using Mapper.Application.Common.Exceptions;
using Mapper.Application.Features.CameraArchive.Commands;
using Mapper.Domain;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Mapper.Tests.CameraArchive.Commands;

public class CreateCameraVideoArchiveCommandHandlerTests : TestCommandBase
{
    private readonly CameraArchiveContextFactory _factory;

    public CreateCameraVideoArchiveCommandHandlerTests() : base(new CameraArchiveContextFactory())
    {
        _factory = (CameraArchiveContextFactory)ContextFactory;
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCreateVideoArchive()
    {
        // Arrange
        var command = new CreateCameraVideoArchiveCommand(
            _factory.CameraMarkId,
            "/videos/test.mp4",
            TimeSpan.FromMinutes(5),
            10485760,
            "1920x1080",
            30,
            "/thumbnails/test.jpg"
        );

        var handler = new CreateCameraVideoArchiveHandler(Context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        var archive = await Context.CameraVideoArchives.FirstOrDefaultAsync(x => x.Id == result);
        Assert.NotNull(archive);
        Assert.Equal(_factory.CameraMarkId, archive.CameraMarkId);
        Assert.Equal("/videos/test.mp4", archive.VideoPath);
        Assert.Equal(TimeSpan.FromMinutes(5), archive.Duration);
        Assert.Equal(10485760, archive.FileSizeBytes);
        Assert.Equal("1920x1080", archive.Resolution);
        Assert.Equal(30, archive.FramesPerSecond);
        Assert.Equal("/thumbnails/test.jpg", archive.ThumbnailPath);
    }

    [Fact]
    public async Task Handle_NonExistentCamera_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new CreateCameraVideoArchiveCommand(
            Guid.NewGuid(),
            "/videos/test.mp4",
            TimeSpan.FromMinutes(1),
            1000000,
            "1280x720",
            25
        );

        var handler = new CreateCameraVideoArchiveHandler(Context);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}

public class MarkVideoArchiveAsArchivedCommandHandlerTests : TestCommandBase
{
    private readonly CameraArchiveContextFactory _factory;

    public MarkVideoArchiveAsArchivedCommandHandlerTests() : base(new CameraArchiveContextFactory())
    {
        _factory = (CameraArchiveContextFactory)ContextFactory;
    }

    private static void SetIdProperty(object obj, Guid id)
    {
        var backingField = obj.GetType().GetField("<Id>k__BackingField",
            BindingFlags.Instance | BindingFlags.NonPublic);
        backingField?.SetValue(obj, id);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldMarkAsArchived()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var archive = new CameraVideoArchive(
            _factory.CameraMarkId,
            "/video.mp4",
            TimeSpan.FromMinutes(1),
            1000000,
            "1920x1080",
            30);
        SetIdProperty(archive, archiveId);

        Context.CameraVideoArchives.Add(archive);
        await Context.SaveChangesAsync();

        var command = new MarkVideoArchiveAsArchivedCommand(archiveId);
        var handler = new MarkVideoArchiveAsArchivedHandler(Context);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var updated = await Context.CameraVideoArchives.FirstOrDefaultAsync(x => x.Id == archiveId);
        Assert.NotNull(updated);
        Assert.True(updated.IsArchived);
    }

    [Fact]
    public async Task Handle_NonExistentArchive_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new MarkVideoArchiveAsArchivedCommand(Guid.NewGuid());
        var handler = new MarkVideoArchiveAsArchivedHandler(Context);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}

public class DeleteCameraVideoArchiveCommandHandlerTests : TestCommandBase
{
    private readonly CameraArchiveContextFactory _factory;

    public DeleteCameraVideoArchiveCommandHandlerTests() : base(new CameraArchiveContextFactory())
    {
        _factory = (CameraArchiveContextFactory)ContextFactory;
    }

    private static void SetIdProperty(object obj, Guid id)
    {
        var backingField = obj.GetType().GetField("<Id>k__BackingField",
            BindingFlags.Instance | BindingFlags.NonPublic);
        backingField?.SetValue(obj, id);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldDeleteArchive()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var archive = new CameraVideoArchive(
            _factory.CameraMarkId,
            "/video.mp4",
            TimeSpan.FromMinutes(1),
            1000000,
            "1920x1080",
            30);
        SetIdProperty(archive, archiveId);

        Context.CameraVideoArchives.Add(archive);
        await Context.SaveChangesAsync();

        var command = new DeleteCameraVideoArchiveCommand(archiveId);
        var handler = new DeleteCameraVideoArchiveHandler(Context);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var deleted = await Context.CameraVideoArchives.FirstOrDefaultAsync(x => x.Id == archiveId);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task Handle_NonExistentArchive_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new DeleteCameraVideoArchiveCommand(Guid.NewGuid());
        var handler = new DeleteCameraVideoArchiveHandler(Context);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}

public class UpdateVideoArchiveMotionDetectionCommandHandlerTests : TestCommandBase
{
    private readonly CameraArchiveContextFactory _factory;

    public UpdateVideoArchiveMotionDetectionCommandHandlerTests() : base(new CameraArchiveContextFactory())
    {
        _factory = (CameraArchiveContextFactory)ContextFactory;
    }

    private static void SetIdProperty(object obj, Guid id)
    {
        var backingField = obj.GetType().GetField("<Id>k__BackingField",
            BindingFlags.Instance | BindingFlags.NonPublic);
        backingField?.SetValue(obj, id);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldUpdateMotionDetection()
    {
        // Arrange
        var archiveId = Guid.NewGuid();
        var archive = new CameraVideoArchive(
            _factory.CameraMarkId,
            "/video.mp4",
            TimeSpan.FromMinutes(1),
            1000000,
            "1920x1080",
            30);
        SetIdProperty(archive, archiveId);

        Context.CameraVideoArchives.Add(archive);
        await Context.SaveChangesAsync();

        var command = new UpdateVideoArchiveMotionDetectionCommand(archiveId, true);
        var handler = new UpdateVideoArchiveMotionDetectionHandler(Context);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var updated = await Context.CameraVideoArchives.FirstOrDefaultAsync(x => x.Id == archiveId);
        Assert.NotNull(updated);
        Assert.True(updated.HasMotionDetected);
    }

    [Fact]
    public async Task Handle_NonExistentArchive_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new UpdateVideoArchiveMotionDetectionCommand(Guid.NewGuid(), true);
        var handler = new UpdateVideoArchiveMotionDetectionHandler(Context);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}

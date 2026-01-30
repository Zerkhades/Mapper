using Mapper.Application.Common.Exceptions;
using Mapper.Application.Features.CameraArchive.Commands;
using Mapper.Domain;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Mapper.Tests.CameraArchive.Commands;

public class CreateCameraMotionAlertCommandHandlerTests : TestCommandBase
{
    private readonly CameraArchiveContextFactory _factory;

    public CreateCameraMotionAlertCommandHandlerTests() : base(new CameraArchiveContextFactory())
    {
        _factory = (CameraArchiveContextFactory)ContextFactory;
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCreateAlert()
    {
        // Arrange
        var command = new CreateCameraMotionAlertCommand(
            _factory.CameraMarkId,
            MotionSeverity.High,
            75.5,
            "/snapshots/alert.jpg"
        );

        var handler = new CreateCameraMotionAlertHandler(Context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        var alert = await Context.CameraMotionAlerts.FirstOrDefaultAsync(x => x.Id == result);
        Assert.NotNull(alert);
        Assert.Equal(_factory.CameraMarkId, alert.CameraMarkId);
        Assert.Equal(MotionSeverity.High, alert.Severity);
        Assert.Equal(75.5, alert.MotionPercentage);
        Assert.Equal("/snapshots/alert.jpg", alert.SnapshotPath);
        Assert.False(alert.IsResolved);
    }

    [Fact]
    public async Task Handle_NonExistentCamera_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new CreateCameraMotionAlertCommand(
            Guid.NewGuid(),
            MotionSeverity.Medium,
            50.0
        );

        var handler = new CreateCameraMotionAlertHandler(Context);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}

public class ConfirmCameraMotionAlertCommandHandlerTests : TestCommandBase
{
    private readonly CameraArchiveContextFactory _factory;

    public ConfirmCameraMotionAlertCommandHandlerTests() : base(new CameraArchiveContextFactory())
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
    public async Task Handle_ValidRequest_ShouldConfirmAlert()
    {
        // Arrange
        var alertId = Guid.NewGuid();
        var alert = new CameraMotionAlert(
            _factory.CameraMarkId,
            MotionSeverity.Medium,
            50.0);
        SetIdProperty(alert, alertId);

        Context.CameraMotionAlerts.Add(alert);
        await Context.SaveChangesAsync();

        var command = new ConfirmCameraMotionAlertCommand(alertId);
        var handler = new ConfirmCameraMotionAlertHandler(Context);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var confirmed = await Context.CameraMotionAlerts.FirstOrDefaultAsync(x => x.Id == alertId);
        Assert.NotNull(confirmed);
        Assert.NotNull(confirmed.ConfirmedAt);
    }

    [Fact]
    public async Task Handle_NonExistentAlert_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new ConfirmCameraMotionAlertCommand(Guid.NewGuid());
        var handler = new ConfirmCameraMotionAlertHandler(Context);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}

public class ResolveCameraMotionAlertCommandHandlerTests : TestCommandBase
{
    private readonly CameraArchiveContextFactory _factory;

    public ResolveCameraMotionAlertCommandHandlerTests() : base(new CameraArchiveContextFactory())
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
    public async Task Handle_ValidRequest_ShouldResolveAlert()
    {
        // Arrange
        var alertId = Guid.NewGuid();
        var alert = new CameraMotionAlert(
            _factory.CameraMarkId,
            MotionSeverity.Low,
            20.0);
        SetIdProperty(alert, alertId);

        Context.CameraMotionAlerts.Add(alert);
        await Context.SaveChangesAsync();

        var notes = "False alarm - maintenance work";
        var command = new ResolveCameraMotionAlertCommand(alertId, notes);
        var handler = new ResolveCameraMotionAlertHandler(Context);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var resolved = await Context.CameraMotionAlerts.FirstOrDefaultAsync(x => x.Id == alertId);
        Assert.NotNull(resolved);
        Assert.True(resolved.IsResolved);
        Assert.Equal(notes, resolved.ResolutionNotes);
    }

    [Fact]
    public async Task Handle_WithoutNotes_ShouldResolveAlert()
    {
        // Arrange
        var alertId = Guid.NewGuid();
        var alert = new CameraMotionAlert(
            _factory.CameraMarkId,
            MotionSeverity.Medium,
            50.0);
        SetIdProperty(alert, alertId);

        Context.CameraMotionAlerts.Add(alert);
        await Context.SaveChangesAsync();

        var command = new ResolveCameraMotionAlertCommand(alertId);
        var handler = new ResolveCameraMotionAlertHandler(Context);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var resolved = await Context.CameraMotionAlerts.FirstOrDefaultAsync(x => x.Id == alertId);
        Assert.NotNull(resolved);
        Assert.True(resolved.IsResolved);
    }

    [Fact]
    public async Task Handle_NonExistentAlert_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new ResolveCameraMotionAlertCommand(Guid.NewGuid(), "notes");
        var handler = new ResolveCameraMotionAlertHandler(Context);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}

public class LinkMotionAlertToVideoCommandHandlerTests : TestCommandBase
{
    private readonly CameraArchiveContextFactory _factory;

    public LinkMotionAlertToVideoCommandHandlerTests() : base(new CameraArchiveContextFactory())
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
    public async Task Handle_ValidRequest_ShouldLinkAlertToVideo()
    {
        // Arrange
        var alertId = Guid.NewGuid();
        var alert = new CameraMotionAlert(
            _factory.CameraMarkId,
            MotionSeverity.High,
            80.0);
        SetIdProperty(alert, alertId);

        var videoId = Guid.NewGuid();
        var video = new CameraVideoArchive(
            _factory.CameraMarkId,
            "/video.mp4",
            TimeSpan.FromMinutes(1),
            1000000,
            "1920x1080",
            30);
        SetIdProperty(video, videoId);

        Context.CameraMotionAlerts.Add(alert);
        Context.CameraVideoArchives.Add(video);
        await Context.SaveChangesAsync();

        var command = new LinkMotionAlertToVideoCommand(alertId, videoId);
        var handler = new LinkMotionAlertToVideoHandler(Context);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var linked = await Context.CameraMotionAlerts.FirstOrDefaultAsync(x => x.Id == alertId);
        Assert.NotNull(linked);
        Assert.Equal(videoId, linked.RelatedVideoArchiveId);
    }

    [Fact]
    public async Task Handle_NonExistentAlert_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new LinkMotionAlertToVideoCommand(Guid.NewGuid(), Guid.NewGuid());
        var handler = new LinkMotionAlertToVideoHandler(Context);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}

public class DeleteCameraMotionAlertCommandHandlerTests : TestCommandBase
{
    private readonly CameraArchiveContextFactory _factory;

    public DeleteCameraMotionAlertCommandHandlerTests() : base(new CameraArchiveContextFactory())
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
    public async Task Handle_ValidRequest_ShouldDeleteAlert()
    {
        // Arrange
        var alertId = Guid.NewGuid();
        var alert = new CameraMotionAlert(
            _factory.CameraMarkId,
            MotionSeverity.Medium,
            50.0);
        SetIdProperty(alert, alertId);

        Context.CameraMotionAlerts.Add(alert);
        await Context.SaveChangesAsync();

        var command = new DeleteCameraMotionAlertCommand(alertId);
        var handler = new DeleteCameraMotionAlertHandler(Context);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var deleted = await Context.CameraMotionAlerts.FirstOrDefaultAsync(x => x.Id == alertId);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task Handle_NonExistentAlert_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new DeleteCameraMotionAlertCommand(Guid.NewGuid());
        var handler = new DeleteCameraMotionAlertHandler(Context);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}

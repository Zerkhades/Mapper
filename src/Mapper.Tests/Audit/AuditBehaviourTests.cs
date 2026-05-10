using Mapper.Application.Common.Behaviours;
using Mapper.Application.Interfaces;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Tests.Audit;

public class AuditBehaviourTests : TestCommandBase
{
    private readonly Guid _userId = Guid.NewGuid();

    public AuditBehaviourTests() : base(new CameraArchiveContextFactory())
    {
    }

    [Fact]
    public async Task Handle_WithCommand_ShouldWriteAuditEvent()
    {
        // Arrange
        var entityId = Guid.NewGuid();
        var request = new CreateWidgetCommand("Test widget");
        var behavior = new AuditBehaviour<CreateWidgetCommand, Guid>(
            Context,
            new TestCurrentUserService(_userId));

        // Act
        var result = await behavior.Handle(
            request,
            _ => Task.FromResult(entityId),
            CancellationToken.None);

        // Assert
        Assert.Equal(entityId, result);
        var auditEvent = await Context.AuditEvents.SingleAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal("Create", auditEvent.Action);
        Assert.Equal("Widget", auditEvent.EntityType);
        Assert.Equal(entityId, auditEvent.EntityId);
        Assert.Equal(_userId, auditEvent.UserId);
        Assert.Contains(nameof(CreateWidgetCommand), auditEvent.MetadataJson);
    }

    [Fact]
    public async Task Handle_WithQuery_ShouldNotWriteAuditEvent()
    {
        // Arrange
        var behavior = new AuditBehaviour<GetWidgetQuery, Guid>(
            Context,
            new TestCurrentUserService(_userId));

        // Act
        await behavior.Handle(
            new GetWidgetQuery(Guid.NewGuid()),
            _ => Task.FromResult(Guid.NewGuid()),
            CancellationToken.None);

        // Assert
        Assert.False(await Context.AuditEvents.AnyAsync(cancellationToken: TestContext.Current.CancellationToken));
    }

    private sealed record CreateWidgetCommand(string Name) : IRequest<Guid>;

    private sealed record GetWidgetQuery(Guid Id) : IRequest<Guid>;

    private sealed class TestCurrentUserService : ICurrentUserService
    {
        public TestCurrentUserService(Guid userId)
        {
            UserId = userId;
        }

        public Guid UserId { get; }
    }
}

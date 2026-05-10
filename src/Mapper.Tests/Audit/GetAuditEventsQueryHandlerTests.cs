using Mapper.Application.Features.Audit.Queries;
using Mapper.Domain;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;

namespace Mapper.Tests.Audit;

public class GetAuditEventsQueryHandlerTests : TestCommandBase
{
    public GetAuditEventsQueryHandlerTests() : base(new CameraArchiveContextFactory())
    {
    }

    [Fact]
    public async Task Handle_WithFilters_ShouldReturnMatchingEventsOrderedByNewest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var targetId = Guid.NewGuid();
        Context.AuditEvents.Add(new AuditEvent(
            "Create",
            "GeoMap",
            targetId,
            userId,
            occurredAt: DateTimeOffset.UtcNow.AddMinutes(-10)));
        Context.AuditEvents.Add(new AuditEvent(
            "Update",
            "GeoMap",
            targetId,
            userId,
            occurredAt: DateTimeOffset.UtcNow.AddMinutes(-1)));
        Context.AuditEvents.Add(new AuditEvent(
            "Delete",
            "Employee",
            Guid.NewGuid(),
            userId,
            occurredAt: DateTimeOffset.UtcNow));
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new GetAuditEventsQueryHandler(Context);

        // Act
        var events = await handler.Handle(
            new GetAuditEventsQuery(EntityType: "GeoMap", EntityId: targetId),
            CancellationToken.None);

        // Assert
        Assert.Equal(2, events.Count);
        Assert.Equal("Update", events[0].Action);
        Assert.Equal("Create", events[1].Action);
        Assert.All(events, auditEvent =>
        {
            Assert.Equal("GeoMap", auditEvent.EntityType);
            Assert.Equal(targetId, auditEvent.EntityId);
        });
    }
}

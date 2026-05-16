using Mapper.Application.Features.Routes.Commands;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Tests.Routes;

public class RouteCommandHandlerTests : TestCommandBase
{
    public RouteCommandHandlerTests() : base(new RoutesContextFactory())
    {
    }

    [Fact]
    public async Task CreateRouteNode_AddsNode()
    {
        // Arrange
        var handler = new CreateRouteNodeCommandHandler(Context);

        // Act
        var nodeId = await handler.Handle(
            new CreateRouteNodeCommand(RoutesContextFactory.GeoMapId, 5, 5, "New node", null),
            TestContext.Current.CancellationToken);

        // Assert
        var node = await Context.RouteNodes.SingleAsync(
            x => x.Id == nodeId,
            TestContext.Current.CancellationToken);
        Assert.Equal("New node", node.Title);
        Assert.Equal(5, node.X);
        Assert.Equal(5, node.Y);
    }

    [Fact]
    public async Task DeleteRouteNode_RemovesConnectedEdges()
    {
        // Arrange
        var handler = new DeleteRouteNodeCommandHandler(Context);

        // Act
        await handler.Handle(
            new DeleteRouteNodeCommand(RoutesContextFactory.GeoMapId, RoutesContextFactory.NodeBId),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.False(await Context.RouteNodes.AnyAsync(
            x => x.Id == RoutesContextFactory.NodeBId,
            TestContext.Current.CancellationToken));
        Assert.False(await Context.RouteEdges.AnyAsync(x =>
            x.FromNodeId == RoutesContextFactory.NodeBId || x.ToNodeId == RoutesContextFactory.NodeBId,
            TestContext.Current.CancellationToken));
    }
}

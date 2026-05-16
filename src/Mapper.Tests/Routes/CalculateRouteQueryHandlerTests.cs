using Mapper.Application.Features.Routes.DTOs;
using Mapper.Application.Features.Routes.Queries;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;

namespace Mapper.Tests.Routes;

public class CalculateRouteQueryHandlerTests : TestCommandBase
{
    public CalculateRouteQueryHandlerTests() : base(new RoutesContextFactory())
    {
    }

    [Fact]
    public async Task CalculateRoute_BetweenArbitraryPoints_ReturnsPolylineWithSnappedGraphPath()
    {
        // Arrange
        var handler = new CalculateRouteQueryHandler(Context);

        // Act
        var result = await handler.Handle(
            new CalculateRouteQuery(
                RoutesContextFactory.GeoMapId,
                new RouteEndpointDto(X: -5, Y: 0),
                new RouteEndpointDto(X: 25, Y: 0),
                WalkingSpeed: 2),
            TestContext.Current.CancellationToken);

        // Assert
        var segment = Assert.Single(result.Segments);
        Assert.Equal(RoutesContextFactory.GeoMapId, segment.GeoMapId);
        Assert.Equal(new[] { -5d, 0d, 10d, 20d, 25d }, segment.Points.Select(x => x.X));
        Assert.Equal(30, result.TotalDistance);
        Assert.Equal(15, result.EstimatedSeconds);
    }

    [Fact]
    public async Task CalculateRoute_FromGeoMark_UsesLinkedRouteNode()
    {
        // Arrange
        var handler = new CalculateRouteQueryHandler(Context);

        // Act
        var result = await handler.Handle(
            new CalculateRouteQuery(
                RoutesContextFactory.GeoMapId,
                new RouteEndpointDto(GeoMarkId: RoutesContextFactory.WorkplaceMarkId),
                new RouteEndpointDto(NodeId: RoutesContextFactory.NodeCId)),
            TestContext.Current.CancellationToken);

        // Assert
        var segment = Assert.Single(result.Segments);
        Assert.Equal(new[] { 10d, 20d }, segment.Points.Select(x => x.X));
    }
}

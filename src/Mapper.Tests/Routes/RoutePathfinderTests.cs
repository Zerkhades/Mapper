using Mapper.Application.Features.Routes;
using Mapper.Domain;

namespace Mapper.Tests.Routes;

public class RoutePathfinderTests
{
    [Fact]
    public void FindPath_ReturnsShortestPathByCost()
    {
        // Arrange
        var mapId = Guid.NewGuid();
        var a = new RouteNode(mapId, 0, 0, "A");
        var b = new RouteNode(mapId, 10, 0, "B");
        var c = new RouteNode(mapId, 20, 0, "C");
        var d = new RouteNode(mapId, 10, 10, "D");

        var edges = new[]
        {
            new RouteEdge(mapId, a.Id, b.Id),
            new RouteEdge(mapId, b.Id, c.Id),
            new RouteEdge(mapId, a.Id, d.Id, costOverride: 50),
            new RouteEdge(mapId, d.Id, c.Id, costOverride: 50)
        };

        // Act
        var path = RoutePathfinder.FindPath(new[] { a, b, c, d }, edges, a.Id, c.Id);

        // Assert
        Assert.Equal(new[] { a.Id, b.Id, c.Id }, path.Select(x => x.Id));
    }

    [Fact]
    public void FindPath_RespectsOneWayEdges()
    {
        // Arrange
        var mapId = Guid.NewGuid();
        var a = new RouteNode(mapId, 0, 0, "A");
        var b = new RouteNode(mapId, 10, 0, "B");
        var edge = new RouteEdge(mapId, a.Id, b.Id, isBidirectional: false);

        // Act
        var path = RoutePathfinder.FindPath(new[] { a, b }, new[] { edge }, b.Id, a.Id);

        // Assert
        Assert.Empty(path);
    }
}

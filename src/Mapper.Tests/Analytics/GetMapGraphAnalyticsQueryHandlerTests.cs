using Mapper.Application.Common.Exceptions;
using Mapper.Application.Features.Analytics.Queries;
using Mapper.Domain;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;

namespace Mapper.Tests.Analytics;

public class GetMapGraphAnalyticsQueryHandlerTests : TestCommandBase
{
    public GetMapGraphAnalyticsQueryHandlerTests() : base(new MapGraphContextFactory())
    {
    }

    [Fact]
    public async Task Handle_WithConnectedAndIsolatedMaps_ShouldReturnGraphSummary()
    {
        // Arrange
        var mapA = AddMap("A");
        var mapB = AddMap("B");
        var mapC = AddMap("C");
        var isolated = AddMap("Isolated");
        AddTransition(mapA.Id, mapB.Id, "A to B");
        AddTransition(mapB.Id, mapC.Id, "B to C");
        await Context.SaveChangesAsync();

        var handler = new GetMapGraphAnalyticsQueryHandler(Context);

        // Act
        var analytics = await handler.Handle(new GetMapGraphAnalyticsQuery(), CancellationToken.None);

        // Assert
        Assert.Equal(4, analytics.MapCount);
        Assert.Equal(2, analytics.TransitionCount);
        Assert.Equal(1, analytics.IsolatedMapCount);
        Assert.Equal(2, analytics.WeakComponentCount);
        Assert.Equal(0.25, analytics.DirectedReachabilityRatio);
        Assert.Contains(analytics.Nodes, x => x.GeoMapId == mapA.Id && x.ReachableMapCount == 2);
        Assert.Contains(analytics.Nodes, x => x.GeoMapId == isolated.Id && x.IsIsolated);
        Assert.Equal(mapA.Id, analytics.Bottlenecks[0].GeoMapId);
    }

    [Fact]
    public async Task Handle_WithSourceAndTarget_ShouldReturnShortestPath()
    {
        // Arrange
        var mapA = AddMap("A");
        var mapB = AddMap("B");
        var mapC = AddMap("C");
        AddTransition(mapA.Id, mapB.Id, "A to B");
        AddTransition(mapB.Id, mapC.Id, "B to C");
        await Context.SaveChangesAsync();

        var handler = new GetMapGraphAnalyticsQueryHandler(Context);

        // Act
        var analytics = await handler.Handle(
            new GetMapGraphAnalyticsQuery(mapA.Id, mapC.Id),
            CancellationToken.None);

        // Assert
        var path = Assert.Single(analytics.ShortestPaths);
        Assert.True(path.Exists);
        Assert.Equal(2, path.Distance);
        Assert.Equal(new[] { mapA.Id, mapB.Id, mapC.Id }, path.Path);
    }

    [Fact]
    public async Task Handle_WithMissingSource_ShouldThrowNotFoundException()
    {
        // Arrange
        var map = AddMap("A");
        await Context.SaveChangesAsync();
        var handler = new GetMapGraphAnalyticsQueryHandler(Context);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new GetMapGraphAnalyticsQuery(Guid.NewGuid(), map.Id), CancellationToken.None));
    }

    private GeoMap AddMap(string name)
    {
        var map = new GeoMap(name, $"/maps/{name}.png", 100, 100);
        Context.GeoMaps.Add(map);
        return map;
    }

    private void AddTransition(Guid sourceGeoMapId, Guid targetGeoMapId, string title)
    {
        Context.GeoMarks.Add(new TransitionMark(
            sourceGeoMapId,
            0.5,
            0.5,
            title,
            targetGeoMapId));
    }
}

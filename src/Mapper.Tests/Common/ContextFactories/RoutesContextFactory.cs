using Mapper.Domain;
using Mapper.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Tests.Common.ContextFactories;

public class RoutesContextFactory : IContextFactory
{
    public static Guid GeoMapId = Guid.NewGuid();
    public static Guid WorkplaceMarkId = Guid.NewGuid();
    public static Guid NodeAId = Guid.NewGuid();
    public static Guid NodeBId = Guid.NewGuid();
    public static Guid NodeCId = Guid.NewGuid();
    public static Guid EdgeABId = Guid.NewGuid();
    public static Guid EdgeBCId = Guid.NewGuid();

    MapperDbContext IContextFactory.Create() => Create();

    void IContextFactory.Destroy(MapperDbContext context) => Destroy(context);

    public static MapperDbContext Create()
    {
        var options = new DbContextOptionsBuilder<MapperDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new MapperDbContext(options);
        context.Database.EnsureCreated();

        var geoMap = CreateGeoMapWithId("Routing Map", "/maps/routing.jpg", 1000, 800, GeoMapId);
        var workplace = CreateWorkplaceMarkWithId(GeoMapId, 10, 0, "Workplace", "WP-ROUTE", WorkplaceMarkId);
        var nodeA = CreateRouteNodeWithId(GeoMapId, 0, 0, "A", null, NodeAId);
        var nodeB = CreateRouteNodeWithId(GeoMapId, 10, 0, "B", WorkplaceMarkId, NodeBId);
        var nodeC = CreateRouteNodeWithId(GeoMapId, 20, 0, "C", null, NodeCId);
        var edgeAB = CreateRouteEdgeWithId(GeoMapId, NodeAId, NodeBId, true, null, EdgeABId);
        var edgeBC = CreateRouteEdgeWithId(GeoMapId, NodeBId, NodeCId, true, null, EdgeBCId);

        context.GeoMaps.Add(geoMap);
        context.GeoMarks.Add(workplace);
        context.RouteNodes.AddRange(nodeA, nodeB, nodeC);
        context.RouteEdges.AddRange(edgeAB, edgeBC);
        context.SaveChanges();

        return context;
    }

    public static void Destroy(MapperDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }

    private static GeoMap CreateGeoMapWithId(string name, string imagePath, int width, int height, Guid id)
    {
        var map = new GeoMap(name, imagePath, width, height);
        typeof(GeoMap).GetProperty("Id")!.SetValue(map, id);
        return map;
    }

    private static WorkplaceMark CreateWorkplaceMarkWithId(
        Guid geoMapId,
        double x,
        double y,
        string title,
        string workplaceCode,
        Guid id)
    {
        var mark = new WorkplaceMark(geoMapId, x, y, title, workplaceCode);
        typeof(GeoMark).GetProperty("Id")!.SetValue(mark, id);
        return mark;
    }

    private static RouteNode CreateRouteNodeWithId(
        Guid geoMapId,
        double x,
        double y,
        string title,
        Guid? geoMarkId,
        Guid id)
    {
        var node = new RouteNode(geoMapId, x, y, title, geoMarkId);
        typeof(RouteNode).GetProperty("Id")!.SetValue(node, id);
        return node;
    }

    private static RouteEdge CreateRouteEdgeWithId(
        Guid geoMapId,
        Guid fromNodeId,
        Guid toNodeId,
        bool isBidirectional,
        double? costOverride,
        Guid id)
    {
        var edge = new RouteEdge(geoMapId, fromNodeId, toNodeId, isBidirectional, costOverride);
        typeof(RouteEdge).GetProperty("Id")!.SetValue(edge, id);
        return edge;
    }
}

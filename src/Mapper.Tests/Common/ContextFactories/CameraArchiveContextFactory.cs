using Mapper.Domain;
using Mapper.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Tests.Common.ContextFactories;

public class CameraArchiveContextFactory : IContextFactory
{
    public Guid GeoMapId { get; private set; }
    public Guid CameraMarkId { get; private set; }

    public MapperDbContext Create()
    {
        var options = new DbContextOptionsBuilder<MapperDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new MapperDbContext(options);
        context.Database.EnsureCreated();

        var geoMap = new GeoMap("Test Map", "/maps/test.png", 1920, 1080, "Test description");

        context.GeoMaps.Add(geoMap);
        context.SaveChanges();

        GeoMapId = geoMap.Id;

        var cameraMark = new CameraMark(
            GeoMapId,
            500.0,
            750.0,
            "Test Camera",
            "CAM-001",
            "rtsp://192.168.1.100/stream",
            "Main entrance camera"
        );

        context.GeoMarks.Add(cameraMark);
        context.SaveChanges();

        CameraMarkId = cameraMark.Id;

        return context;
    }

    public void Destroy(MapperDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }
}

using Microsoft.EntityFrameworkCore;
using Mapper.Domain;
using Mapper.Persistence;

namespace Mapper.Tests.Common.ContextFactories
{
    public class GeoMapsContextFactory : IContextFactory
    {
        public static Guid GeoMapIdForDelete = Guid.NewGuid();

        MapperDbContext IContextFactory.Create()
        {
            return Create();
        }

        void IContextFactory.Destroy(MapperDbContext context)
        {
            Destroy(context);
        }

        public static MapperDbContext Create()
        {
            var options = new DbContextOptionsBuilder<MapperDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new MapperDbContext(options);
            context.Database.EnsureCreated();

            var mapForDelete = new GeoMap("GeoMapForDelete", "/maps/map1.jpg", 1920, 1080, "Description for delete");
            typeof(GeoMap).GetProperty("Id")!.SetValue(mapForDelete, GeoMapIdForDelete);

            context.GeoMaps.Add(mapForDelete);
            context.SaveChanges();
            return context;
        }

        public static void Destroy(MapperDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}

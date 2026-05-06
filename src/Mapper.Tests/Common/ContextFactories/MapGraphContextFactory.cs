using Mapper.Domain;
using Mapper.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Tests.Common.ContextFactories;

public class MapGraphContextFactory : IContextFactory
{
    public MapperDbContext Create()
    {
        var options = new DbContextOptionsBuilder<MapperDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new MapperDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public void Destroy(MapperDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }
}

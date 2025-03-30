using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapper.Domain;
using Mapper.Persistence;

namespace Mapper.Tests.Common.ContextFactories
{
    public class GeoMapsContextFactory : IContextFactory
    {
        public static Guid GeoMapIdForCreate = Guid.NewGuid();
        public static Guid GeoMapIdForDelete = Guid.NewGuid();
        public static Guid GeoMapIdForUpdate = Guid.NewGuid();

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
            context.GeoMaps.AddRange(
                new GeoMap
                {
                    Id = GeoMapIdForCreate,
                    MapName = "string",
                    MapDescription = "string",
                    IsArchived = false
                    
                },
                new GeoMap
                {
                    Id = GeoMapIdForUpdate,
                    MapName = "string",
                    MapDescription = "string",
                    IsArchived = false
                },
                new GeoMap
                {
                    Id = GeoMapIdForDelete,
                    MapName = "string",
                    MapDescription = "string",
                    IsArchived = false
                }
            );
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

using Mapper.Domain;
using Mapper.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Tests.Common.ContextFactories
{
    public class EmployeesContextFactory : IContextFactory
    {
        public static Guid GeoMapIdForCreate = Guid.NewGuid();
        public static Guid GeoMapIdForUpdate = Guid.NewGuid();
        public static Guid GeoMapIdForDelete = Guid.NewGuid();

        public static Guid GeoMarkIdForCreate = Guid.NewGuid();
        public static Guid GeoMarkIdForUpdate = Guid.NewGuid();
        public static Guid GeoMarkIdForDelete = Guid.NewGuid();

        public static Guid EmployeeIdForCreate = Guid.NewGuid();
        public static Guid EmployeeIdForUpdate = Guid.NewGuid();
        public static Guid EmployeeIdForDelete = Guid.NewGuid();

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
                // Create
                new GeoMap
                {
                    Id = GeoMapIdForCreate,
                    MapName = "geomap1",
                    MapDescription = "geomap1",
                    IsArchived = false
                },
                // Update/Archive
                new GeoMap
                {
                    Id = GeoMapIdForUpdate,
                    MapName = "geomap2",
                    MapDescription = "geomap2",
                    IsArchived = false,
                    GeoMarks =
                    [
                        new GeoMark()
                        {
                            Id = GeoMarkIdForUpdate,
                            MarkName = "GeoMark2",
                            IsArchived = false,
                            Employees =
                            [
                                new Employee()
                                {
                                    Id = EmployeeIdForDelete,
                                    FirstName = "Employee2",
                                    Surname = "Employee2",
                                    IsArchived = false,

                                }
                            ]
                        }
                    ]
                },
                // Delete
                new GeoMap
                {
                    Id = GeoMapIdForDelete,
                    MapName = "GeoMap3",
                    MapDescription = "GeoMap3",
                    IsArchived = false,
                    GeoMarks =
                    [
                        new GeoMark()
                        {
                            Id = GeoMarkIdForDelete,
                            GeoMapId = GeoMapIdForDelete,
                            MarkName = "test",
                        }
                    ]
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

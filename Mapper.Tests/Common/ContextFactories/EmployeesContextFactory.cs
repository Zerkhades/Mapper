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
        public static Guid GeoMapIdForArchive = Guid.NewGuid();

        public static Guid GeoMarkIdForCreate = Guid.NewGuid();
        public static Guid GeoMarkIdForUpdate = Guid.NewGuid();
        public static Guid GeoMarkIdForDelete = Guid.NewGuid();
        public static Guid GeoMarkIdForArchive = Guid.NewGuid();

        public static Guid EmployeeIdForCreate = Guid.NewGuid();
        public static Guid EmployeeIdForUpdate = Guid.NewGuid();
        public static Guid EmployeeIdForDelete = Guid.NewGuid();
        public static Guid EmployeeIdForArchive = Guid.NewGuid();

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
                    MapName = "GeoMapForCreate",
                    MapDescription = "GeoMapForCreate",
                    IsArchived = false,
                    GeoMarks =
                    [
                        new GeoMark()
                        {
                            Id = GeoMarkIdForCreate,
                            MarkName = "GeoMarkForCreate",
                            Employees =
                            [
                                new Employee()
                                {
                                    Id = EmployeeIdForCreate,
                                    FirstName = "John",
                                    Surname = "Doe",
                                    Email = "john.doe@example.com",
                                    IsArchived = false,
                                }
                            ]
                        }
                    ]

                },
                // Update
                new GeoMap
                {
                    Id = GeoMapIdForUpdate,
                    MapName = "GeoMapForUpdate",
                    MapDescription = "GeoMapForUpdate",
                    IsArchived = false,
                    GeoMarks =
                    [
                        new GeoMark()
                        {
                            Id = GeoMarkIdForUpdate,
                            MarkName = "GeoMarkForUpdate",
                            Employees =
                            [
                                new Employee()
                                {
                                    Id = EmployeeIdForUpdate,
                                    FirstName = "EmployeeForUpdate",
                                    Surname = "EmployeeForUpdate",
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
                    MapName = "GeoMapForDelete",
                    MapDescription = "GeoMapForDelete",
                    IsArchived = false,
                    GeoMarks =
                    [
                        new GeoMark()
                        {
                            Id = GeoMarkIdForDelete,
                            MarkName = "GeoMarkForDelete",
                            Employees =
                            [
                                new Employee()
                                {
                                    Id = EmployeeIdForDelete,
                                    FirstName = "EmployeeForDelete",
                                    Surname = "EmployeeForDelete",
                                    IsArchived = false,
                                }
                            ]
                        }
                    ]
                },
                // Archive
                new GeoMap
                {
                    Id = GeoMapIdForArchive,
                    MapName = "GeoMapForArchive",
                    MapDescription = "GeoMapForArchive",
                    IsArchived = false,
                    GeoMarks =
                    [
                        new GeoMark()
                        {
                            Id = GeoMarkIdForArchive,
                            MarkName = "GeoMarkForArchive",
                            Employees =
                            [
                                new Employee()
                                {
                                    Id = EmployeeIdForArchive,
                                    FirstName = "EmployeeForArchive",
                                    Surname = "EmployeeForArchive",
                                    IsArchived = false,
                                }
                            ]
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

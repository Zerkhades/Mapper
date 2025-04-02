using Mapper.Domain;
using Mapper.Persistence;
using Microsoft.EntityFrameworkCore;

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
                    GeoMarks = new List<GeoMark>
                    {
                            new GeoMark()
                            {
                                Id = GeoMarkIdForCreate,
                                MarkName = "GeoMarkForCreate",
                                Employees = new List<Employee>
                                {
                                    new Employee()
                                    {
                                        Id = EmployeeIdForCreate,
                                        FirstName = "John",
                                        Surname = "Doe",
                                        Email = "john.doe@example.com",
                                        IsArchived = false,
                                        GeoMarkId = GeoMarkIdForCreate
                                    }
                                }
                            }
                    }
                },
                // Update
                new GeoMap
                {
                    Id = GeoMapIdForUpdate,
                    MapName = "GeoMapForUpdate",
                    MapDescription = "GeoMapForUpdate",
                    IsArchived = false,
                    GeoMarks = new List<GeoMark>
                    {
                            new GeoMark()
                            {
                                Id = GeoMarkIdForUpdate,
                                MarkName = "GeoMarkForUpdate",
                                Employees = new List<Employee>
                                {
                                    new Employee()
                                    {
                                        Id = EmployeeIdForUpdate,
                                        FirstName = "EmployeeForUpdate",
                                        Surname = "EmployeeForUpdate",
                                        IsArchived = false,
                                        GeoMarkId = GeoMarkIdForUpdate
                                    }
                                }
                            }
                    }
                },
                // Delete
                new GeoMap
                {
                    Id = GeoMapIdForDelete,
                    MapName = "GeoMapForDelete",
                    MapDescription = "GeoMapForDelete",
                    IsArchived = false,
                    GeoMarks = new List<GeoMark>
                    {
                            new GeoMark()
                            {
                                Id = GeoMarkIdForDelete,
                                MarkName = "GeoMarkForDelete",
                                Employees = new List<Employee>
                                {
                                    new Employee()
                                    {
                                        Id = EmployeeIdForDelete,
                                        FirstName = "EmployeeForDelete",
                                        Surname = "EmployeeForDelete",
                                        IsArchived = false,
                                        GeoMarkId = GeoMarkIdForDelete
                                    }
                                }
                            }
                    }
                },
                // Archive
                new GeoMap
                {
                    Id = GeoMapIdForArchive,
                    MapName = "GeoMapForArchive",
                    MapDescription = "GeoMapForArchive",
                    IsArchived = false,
                    GeoMarks = new List<GeoMark>
                    {
                            new GeoMark()
                            {
                                Id = GeoMarkIdForArchive,
                                MarkName = "GeoMarkForArchive",
                                Employees = new List<Employee>
                                {
                                    new Employee()
                                    {
                                        Id = EmployeeIdForArchive,
                                        FirstName = "EmployeeForArchive",
                                        Surname = "EmployeeForArchive",
                                        IsArchived = false,
                                        GeoMarkId = GeoMarkIdForArchive,
                                    }
                                }
                            }
                    }
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

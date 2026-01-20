using Mapper.Domain;
using Mapper.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Tests.Common.ContextFactories
{
    public class EmployeesContextFactory : IContextFactory
    {
        public static Guid GeoMapId = Guid.NewGuid();
        public static Guid WorkplaceMarkId = Guid.NewGuid();
        public static Guid AnotherWorkplaceMarkId = Guid.NewGuid();
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

            var geoMap = CreateGeoMapWithId("Test Map for Employees", "/maps/employees.jpg", 1920, 1080, "Employee test map", GeoMapId);
            context.GeoMaps.Add(geoMap);

            var workplaceMark = CreateWorkplaceMarkWithId(GeoMapId, 0.5, 0.5, "Main Workplace", "WP-MAIN", "Main workplace", WorkplaceMarkId);
            var anotherWorkplaceMark = CreateWorkplaceMarkWithId(GeoMapId, 0.6, 0.6, "Another Workplace", "WP-OTHER", "Another workplace", AnotherWorkplaceMarkId);
            
            context.GeoMarks.AddRange(workplaceMark, anotherWorkplaceMark);

            var employee1 = new Employee
            {
                Id = EmployeeIdForUpdate,
                FirstName = "John",
                Surname = "Doe",
                Patronymic = "Smith",
                Phone = "+1234567890",
                Email = "john.doe@test.com",
                Cabinet = "101",
                Comment = "Test employee for update",
                GeoMarkId = WorkplaceMarkId,
                IsArchived = false
            };

            var employee2 = new Employee
            {
                Id = EmployeeIdForDelete,
                FirstName = "Jane",
                Surname = "Smith",
                Phone = "+0987654321",
                Email = "jane.smith@test.com",
                GeoMarkId = WorkplaceMarkId,
                IsArchived = false
            };

            context.Employees.AddRange(employee1, employee2);
            context.SaveChanges();
            return context;
        }

        public static void Destroy(MapperDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        private static GeoMap CreateGeoMapWithId(string name, string imagePath, int width, int height, string? description, Guid id)
        {
            var map = new GeoMap(name, imagePath, width, height, description);
            typeof(GeoMap).GetProperty("Id")!.SetValue(map, id);
            return map;
        }

        private static WorkplaceMark CreateWorkplaceMarkWithId(Guid geoMapId, double x, double y, string title, string workplaceCode, string? description, Guid id)
        {
            var mark = new WorkplaceMark(geoMapId, x, y, title, workplaceCode, description);
            typeof(GeoMark).GetProperty("Id")!.SetValue(mark, id);
            return mark;
        }
    }
}

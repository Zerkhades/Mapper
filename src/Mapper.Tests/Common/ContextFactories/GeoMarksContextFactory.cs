using Mapper.Domain;
using Mapper.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Mapper.Tests.Common.ContextFactories
{
    public class GeoMarksContextFactory : IContextFactory
    {
        public static Guid GeoMapId = Guid.NewGuid();
        public static Guid TargetGeoMapId = Guid.NewGuid();
        public static Guid TransitionMarkId = Guid.NewGuid();
        public static Guid WorkplaceMarkId = Guid.NewGuid();
        public static Guid CameraMarkId = Guid.NewGuid();

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

            var geoMap = CreateGeoMapWithId("Test Map", "/maps/test.jpg", 1920, 1080, "Test map for marks", GeoMapId);
            var targetMap = CreateGeoMapWithId("Target Map", "/maps/target.jpg", 1920, 1080, "Target map", TargetGeoMapId);

            context.GeoMaps.AddRange(geoMap, targetMap);

            var transitionMark = CreateTransitionMarkWithId(GeoMapId, 0.5, 0.5, "Transition to Target", TargetGeoMapId, "Test transition", TransitionMarkId);
            var workplaceMark = CreateWorkplaceMarkWithId(GeoMapId, 0.3, 0.4, "Workplace 1", "WP-001", "Test workplace", WorkplaceMarkId);
            var cameraMark = CreateCameraMarkWithId(GeoMapId, 0.7, 0.8, "Camera 1", "CAM-001", "rtsp://test.com/stream", "Test camera", CameraMarkId);

            context.GeoMarks.AddRange(transitionMark, workplaceMark, cameraMark);

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

        private static TransitionMark CreateTransitionMarkWithId(Guid geoMapId, double x, double y, string title, Guid targetGeoMapId, string? description, Guid id)
        {
            var mark = new TransitionMark(geoMapId, x, y, title, targetGeoMapId, description);
            typeof(GeoMark).GetProperty("Id")!.SetValue(mark, id);
            return mark;
        }

        private static WorkplaceMark CreateWorkplaceMarkWithId(Guid geoMapId, double x, double y, string title, string workplaceCode, string? description, Guid id)
        {
            var mark = new WorkplaceMark(geoMapId, x, y, title, workplaceCode, description);
            typeof(GeoMark).GetProperty("Id")!.SetValue(mark, id);
            return mark;
        }

        private static CameraMark CreateCameraMarkWithId(Guid geoMapId, double x, double y, string title, string? cameraName, string? streamUrl, string? description, Guid id)
        {
            var mark = new CameraMark(geoMapId, x, y, title, cameraName, streamUrl, description);
            typeof(GeoMark).GetProperty("Id")!.SetValue(mark, id);
            return mark;
        }
    }
}

namespace Mapper.WebApi.Models.Marks
{
    public class CreateTransitionMarkRequest
    {
        public double X { get; set; }          // 0..1
        public double Y { get; set; }          // 0..1
        public string Title { get; set; } = default!;
        public string? Description { get; set; }

        public Guid TargetGeoMapId { get; set; }
    }

    public class CreateWorkplaceMarkRequest
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }

        public string WorkplaceCode { get; set; } = default!;
        public List<Guid>? EmployeeIds { get; set; }
    }

    public class CreateCameraMarkRequest
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }

        public string? CameraName { get; set; }
        public string? StreamUrl { get; set; }
    }
}

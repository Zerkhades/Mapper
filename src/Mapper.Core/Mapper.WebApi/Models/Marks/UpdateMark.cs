namespace Mapper.WebApi.Models.Marks
{
    public class UpdateTransitionMarkRequest
    {
        public float X { get; set; }
        public float Y { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public Guid TargetGeoMapId { get; set; }
    }

    public class UpdateWorkplaceMarkRequest
    {
        public float X { get; set; }
        public float Y { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }

        public string WorkplaceCode { get; set; } = default!;
        public List<Guid>? EmployeeIds { get; set; }
    }

    public class UpdateCameraMarkRequest
    {
        public float X { get; set; }
        public float Y { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }

        public string? CameraName { get; set; }
        public string? StreamUrl { get; set; }
    }
}

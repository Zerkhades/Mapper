namespace Mapper.Application.Features.DTOs;

public class CameraMotionAlertDto
{
    public Guid Id { get; set; }
    public Guid CameraMarkId { get; set; }
    public DateTimeOffset DetectedAt { get; set; }
    public DateTimeOffset? ConfirmedAt { get; set; }
    public string Severity { get; set; } = default!;
    public double MotionPercentage { get; set; }
    public string? SnapshotUrl { get; set; }
    public bool IsResolved { get; set; }
    public string? ResolutionNotes { get; set; }
    public Guid? RelatedVideoArchiveId { get; set; }
}

public class CameraMotionAlertListItemDto
{
    public Guid Id { get; set; }
    public DateTimeOffset DetectedAt { get; set; }
    public string Severity { get; set; } = default!;
    public double MotionPercentage { get; set; }
    public bool IsResolved { get; set; }
}

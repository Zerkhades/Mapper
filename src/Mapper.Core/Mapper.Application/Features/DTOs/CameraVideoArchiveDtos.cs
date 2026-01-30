namespace Mapper.Application.Features.DTOs;

public class CameraVideoArchiveDto
{
    public Guid Id { get; set; }
    public Guid CameraMarkId { get; set; }
    public string VideoPath { get; set; } = default!;
    public string? ThumbnailPath { get; set; }
    public DateTimeOffset RecordedAt { get; set; }
    public TimeSpan Duration { get; set; }
    public long FileSizeBytes { get; set; }
    public bool HasMotionDetected { get; set; }
    public string Resolution { get; set; } = default!;
    public int FramesPerSecond { get; set; }
    public bool IsArchived { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class CameraVideoArchiveListItemDto
{
    public Guid Id { get; set; }
    public DateTimeOffset RecordedAt { get; set; }
    public TimeSpan Duration { get; set; }
    public string? ThumbnailUrl { get; set; }
    public bool HasMotionDetected { get; set; }
    public string Resolution { get; set; } = default!;
}

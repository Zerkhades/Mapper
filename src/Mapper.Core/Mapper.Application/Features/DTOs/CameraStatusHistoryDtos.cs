namespace Mapper.Application.Features.DTOs;

public class CameraStatusHistoryDto
{
    public Guid Id { get; set; }
    public Guid CameraMarkId { get; set; }
    public bool IsOnline { get; set; }
    public string Reason { get; set; } = default!;
    public DateTimeOffset ChangedAt { get; set; }
    public TimeSpan? DurationSinceLastChange { get; set; }
    public string? Details { get; set; }
    public int? ResponseTimeMs { get; set; }
}

public class CameraStatusHistoryListItemDto
{
    public Guid Id { get; set; }
    public DateTimeOffset ChangedAt { get; set; }
    public bool IsOnline { get; set; }
    public string Reason { get; set; } = default!;
    public TimeSpan? DurationSinceLastChange { get; set; }
}

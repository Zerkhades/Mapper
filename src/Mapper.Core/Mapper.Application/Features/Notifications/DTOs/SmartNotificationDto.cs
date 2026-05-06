namespace Mapper.Application.Features.Notifications.DTOs;

public class SmartNotificationDto
{
    public string Id { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string Severity { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Message { get; set; } = default!;
    public Guid? GeoMapId { get; set; }
    public Guid? CameraMarkId { get; set; }
    public string? CameraTitle { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public DateTimeOffset GeneratedAt { get; set; }
    public Dictionary<string, string> Context { get; set; } = [];
}

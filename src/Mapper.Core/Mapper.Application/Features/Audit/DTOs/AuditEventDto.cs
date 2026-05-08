namespace Mapper.Application.Features.Audit.DTOs;

public class AuditEventDto
{
    public Guid Id { get; set; }
    public string Action { get; set; } = default!;
    public string EntityType { get; set; } = default!;
    public Guid? EntityId { get; set; }
    public Guid? UserId { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public string? MetadataJson { get; set; }
}

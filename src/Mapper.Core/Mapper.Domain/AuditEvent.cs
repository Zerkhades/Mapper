namespace Mapper.Domain;

public class AuditEvent
{
    public Guid Id { get; private init; } = Guid.NewGuid();
    public string Action { get; private set; } = default!;
    public string EntityType { get; private set; } = default!;
    public Guid? EntityId { get; private set; }
    public Guid? UserId { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public string? MetadataJson { get; private set; }

    private AuditEvent() { } // EF

    public AuditEvent(
        string action,
        string entityType,
        Guid? entityId,
        Guid? userId,
        string? metadataJson = null,
        DateTimeOffset? occurredAt = null)
    {
        Action = action;
        EntityType = entityType;
        EntityId = entityId;
        UserId = userId == Guid.Empty ? null : userId;
        MetadataJson = metadataJson;
        OccurredAt = occurredAt ?? DateTimeOffset.UtcNow;
    }
}

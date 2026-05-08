using Mapper.Application.Features.Audit.DTOs;
using MediatR;

namespace Mapper.Application.Features.Audit.Queries;

public record GetAuditEventsQuery(
    string? EntityType = null,
    Guid? EntityId = null,
    Guid? UserId = null,
    DateTimeOffset? From = null,
    DateTimeOffset? To = null,
    int Skip = 0,
    int Take = 100)
    : IRequest<List<AuditEventDto>>;

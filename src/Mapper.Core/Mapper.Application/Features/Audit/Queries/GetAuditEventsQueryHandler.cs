using Mapper.Application.Features.Audit.DTOs;
using Mapper.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.Audit.Queries;

public class GetAuditEventsQueryHandler
    : IRequestHandler<GetAuditEventsQuery, List<AuditEventDto>>
{
    private readonly IMapperDbContext _db;

    public GetAuditEventsQueryHandler(IMapperDbContext db)
    {
        _db = db;
    }

    public async Task<List<AuditEventDto>> Handle(GetAuditEventsQuery request, CancellationToken ct)
    {
        var take = Math.Clamp(request.Take, 1, 500);
        var skip = Math.Max(0, request.Skip);

        var query = _db.AuditEvents.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.EntityType))
        {
            query = query.Where(x => x.EntityType == request.EntityType);
        }

        if (request.EntityId.HasValue)
        {
            query = query.Where(x => x.EntityId == request.EntityId);
        }

        if (request.UserId.HasValue)
        {
            query = query.Where(x => x.UserId == request.UserId);
        }

        if (request.From.HasValue)
        {
            query = query.Where(x => x.OccurredAt >= request.From.Value);
        }

        if (request.To.HasValue)
        {
            query = query.Where(x => x.OccurredAt < request.To.Value);
        }

        return await query
            .OrderByDescending(x => x.OccurredAt)
            .Skip(skip)
            .Take(take)
            .Select(x => new AuditEventDto
            {
                Id = x.Id,
                Action = x.Action,
                EntityType = x.EntityType,
                EntityId = x.EntityId,
                UserId = x.UserId,
                OccurredAt = x.OccurredAt,
                MetadataJson = x.MetadataJson
            })
            .ToListAsync(ct);
    }
}

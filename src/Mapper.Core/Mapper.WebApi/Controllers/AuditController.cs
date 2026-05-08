using Asp.Versioning;
using Mapper.Application.Features.Audit.DTOs;
using Mapper.Application.Features.Audit.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Mapper.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/audit/events")]
public class AuditController : BaseController
{
    [HttpGet]
    public async Task<ActionResult<List<AuditEventDto>>> GetEvents(
        [FromQuery] string? entityType,
        [FromQuery] Guid? entityId,
        [FromQuery] Guid? userId,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 100,
        CancellationToken ct = default)
    {
        var events = await Mediator.Send(
            new GetAuditEventsQuery(entityType, entityId, userId, from, to, skip, take),
            ct);

        return Ok(events);
    }
}

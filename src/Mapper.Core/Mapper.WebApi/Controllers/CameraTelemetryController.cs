using Asp.Versioning;
using Mapper.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mapper.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/geomaps/{geoMapId:guid}/marks/camera/{markId:guid}")]
public class CameraTelemetryController : ControllerBase
{
    private readonly ICacheService _cache;

    public CameraTelemetryController(ICacheService cache) => _cache = cache;

    //[Authorize]
    [HttpGet("status")]
    public async Task<ActionResult<object>> GetStatus(Guid geoMapId, Guid markId, CancellationToken ct)
    {
        var status = await _cache.GetAsync<string>($"camera:{markId}:status", ct) ?? "unknown";
        return Ok(new { markId, status });
    }

    //[Authorize]
    [HttpGet("snapshot")]
    public async Task<ActionResult<object>> GetSnapshot(Guid geoMapId, Guid markId, CancellationToken ct)
    {
        var key = await _cache.GetAsync<string>($"camera:{markId}:snapshotKey", ct);
        if (string.IsNullOrWhiteSpace(key))
            return Ok(new { markId, snapshotUrl = (string?)null });

        return Ok(new { markId, snapshotUrl = $"/api/files/{key}" });
    }
}

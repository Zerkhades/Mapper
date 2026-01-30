using Asp.Versioning;
using Mapper.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mapper.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/geomaps/{geoMapId:guid}/marks/camera/{markId:guid}")]
public class CameraTelemetryController : BaseController
{
    private readonly ICacheService _cache;
    private readonly ICameraAdapter _cameraAdapter;

    public CameraTelemetryController(ICacheService cache, ICameraAdapter cameraAdapter) 
    { 
        _cache = cache;
        _cameraAdapter = cameraAdapter;
    }

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

    // ============== ZOOM/SNAPSHOT WITH ZOOM ==============

    [HttpGet("snapshot/zoom")]
    public async Task<ActionResult<object>> GetSnapshotWithZoom(
        Guid geoMapId, 
        Guid markId,
        [FromQuery] double zoom = 1.0,
        [FromQuery] int? centerX = null,
        [FromQuery] int? centerY = null,
        CancellationToken ct = default)
    {
        if (zoom < 1.0 || zoom > 10.0)
            return BadRequest(new { error = "Zoom level must be between 1.0 and 10.0" });

        // Get the camera stream URL from cache or database
        var streamUrl = await _cache.GetAsync<string>($"camera:{markId}:streamUrl", ct);
        
        if (string.IsNullOrWhiteSpace(streamUrl))
            return NotFound(new { error = "Camera stream URL not found" });

        try
        {
            var snapshot = await _cameraAdapter.TryGetSnapshotWithZoomAsync(
                streamUrl, 
                zoom, 
                centerX, 
                centerY, 
                ct);

            if (snapshot is null)
                return StatusCode(502, new { error = "Failed to capture zoomed snapshot from camera" });

            var cacheKey = $"camera:{markId}:snapshot_zoom_{zoom:F1}";
            await _cache.SetAsync(cacheKey, snapshot.FileName, TimeSpan.FromHours(1), ct);

            return Ok(new
            {
                markId,
                zoom,
                centerX,
                centerY,
                snapshotUrl = $"/api/files/{cacheKey}",
                contentType = snapshot.ContentType
            });
        }
        catch (Exception ex)
        {
            return StatusCode(502, new { error = "Failed to get zoomed snapshot", details = ex.Message });
        }
    }
}

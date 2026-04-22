using Asp.Versioning;
using Mapper.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Mapper.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/geomaps/{geoMapId:guid}/marks/camera/{markId:guid}")]
public class CameraTelemetryController : BaseController
{
    private readonly ICacheService _cache;
    private readonly ICameraAdapter _cameraAdapter;
    private readonly IS3ObjectStorage _storage;

    public CameraTelemetryController(ICacheService cache, ICameraAdapter cameraAdapter, IS3ObjectStorage storage)
    { 
        _cache = cache;
        _cameraAdapter = cameraAdapter;
        _storage = storage;
    }

    [HttpGet("status")]
    public async Task<ActionResult<object>> GetStatus(Guid geoMapId, Guid markId, CancellationToken ct)
    {
        var status = await _cache.GetAsync<string>($"camera:{markId}:status", ct) ?? "unknown";
        return Ok(new { markId, status });
    }

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

            var zoomSegment = zoom.ToString("0.0", CultureInfo.InvariantCulture).Replace('.', '_');
            var centerXSegment = centerX?.ToString(CultureInfo.InvariantCulture) ?? "auto";
            var centerYSegment = centerY?.ToString(CultureInfo.InvariantCulture) ?? "auto";
            var objectKey = $"cameras/{markId}/zoom/{zoomSegment}_{centerXSegment}_{centerYSegment}.png";

            await using var stream = new MemoryStream(snapshot.Bytes);
            await _storage.PutAsync(objectKey, stream, snapshot.ContentType, ct);

            var cacheKey = $"camera:{markId}:snapshot_zoom_{zoomSegment}:{centerXSegment}:{centerYSegment}";
            await _cache.SetAsync(cacheKey, objectKey, TimeSpan.FromHours(1), ct);

            return Ok(new
            {
                markId,
                zoom,
                centerX,
                centerY,
                snapshotUrl = $"/api/files/{objectKey}",
                contentType = snapshot.ContentType
            });
        }
        catch (Exception ex)
        {
            return StatusCode(502, new { error = "Failed to get zoomed snapshot", details = ex.Message });
        }
    }
}

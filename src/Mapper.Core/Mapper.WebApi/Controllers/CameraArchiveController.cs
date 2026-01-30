using Asp.Versioning;
using Mapper.Application.Features.CameraArchive.Commands;
using Mapper.Application.Features.CameraArchive.Queries;
using Mapper.Application.Features.DTOs;
using Mapper.Domain;
using Mapper.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Mapper.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/geomaps/{geoMapId:guid}/marks/camera/{cameraMarkId:guid}/archive")]
public class CameraArchiveController : BaseController
{
    // ============== VIDEO ARCHIVE ==============
    
    [HttpGet("videos")]
    public async Task<ActionResult<List<CameraVideoArchiveListItemDto>>> GetVideoArchive(
        Guid geoMapId, 
        Guid cameraMarkId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50,
        CancellationToken ct = default)
    {
        var videos = await Mediator.Send(new GetCameraVideoArchiveQuery(cameraMarkId, skip, take), ct);
        return Ok(videos);
    }

    [HttpGet("videos/{videoId:guid}")]
    public async Task<ActionResult<CameraVideoArchiveDto>> GetVideoById(
        Guid geoMapId,
        Guid cameraMarkId,
        Guid videoId,
        CancellationToken ct = default)
    {
        var video = await Mediator.Send(new GetCameraVideoArchiveByIdQuery(cameraMarkId, videoId), ct);
        return Ok(video);
    }

    [HttpGet("videos/timeline")]
    public async Task<ActionResult<List<CameraVideoArchiveListItemDto>>> GetVideoTimeline(
        Guid geoMapId,
        Guid cameraMarkId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken ct = default)
    {
        var videos = await Mediator.Send(
            new GetCameraVideoArchiveTimelineQuery(cameraMarkId, startDate, endDate), ct);
        return Ok(videos);
    }

    [HttpPost("videos")]
    public async Task<ActionResult<Guid>> CreateVideoArchive(
        Guid geoMapId,
        Guid cameraMarkId,
        [FromBody] CreateVideoArchiveRequest request,
        CancellationToken ct = default)
    {
        var id = await Mediator.Send(new CreateCameraVideoArchiveCommand(
            cameraMarkId,
            request.VideoPath,
            request.Duration,
            request.FileSizeBytes,
            request.Resolution,
            request.FramesPerSecond,
            request.ThumbnailPath
        ), ct);
        return Ok(id);
    }

    [HttpPut("videos/{videoId:guid}/mark-archived")]
    public async Task<IActionResult> MarkVideoArchived(
        Guid geoMapId,
        Guid cameraMarkId,
        Guid videoId,
        CancellationToken ct = default)
    {
        await Mediator.Send(new MarkVideoArchiveAsArchivedCommand(videoId), ct);
        return NoContent();
    }

    [HttpDelete("videos/{videoId:guid}")]
    public async Task<IActionResult> DeleteVideo(
        Guid geoMapId,
        Guid cameraMarkId,
        Guid videoId,
        CancellationToken ct = default)
    {
        await Mediator.Send(new DeleteCameraVideoArchiveCommand(videoId), ct);
        return NoContent();
    }

    // ============== MOTION ALERTS ==============

    [HttpGet("motion-alerts")]
    public async Task<ActionResult<List<CameraMotionAlertListItemDto>>> GetMotionAlerts(
        Guid geoMapId,
        Guid cameraMarkId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50,
        CancellationToken ct = default)
    {
        var alerts = await Mediator.Send(new GetCameraMotionAlertsQuery(cameraMarkId, skip, take), ct);
        return Ok(alerts);
    }

    [HttpGet("motion-alerts/{alertId:guid}")]
    public async Task<ActionResult<CameraMotionAlertDto>> GetMotionAlertById(
        Guid geoMapId,
        Guid cameraMarkId,
        Guid alertId,
        CancellationToken ct = default)
    {
        var alert = await Mediator.Send(new GetCameraMotionAlertByIdQuery(cameraMarkId, alertId), ct);
        return Ok(alert);
    }

    [HttpGet("motion-alerts/unresolved")]
    public async Task<ActionResult<List<CameraMotionAlertListItemDto>>> GetUnresolvedMotionAlerts(
        Guid geoMapId,
        Guid cameraMarkId,
        CancellationToken ct = default)
    {
        var alerts = await Mediator.Send(new GetUnresolvedCameraMotionAlertsQuery(cameraMarkId), ct);
        return Ok(alerts);
    }

    [HttpGet("motion-alerts/timeline")]
    public async Task<ActionResult<List<CameraMotionAlertListItemDto>>> GetMotionAlertsTimeline(
        Guid geoMapId,
        Guid cameraMarkId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken ct = default)
    {
        var alerts = await Mediator.Send(
            new GetCameraMotionAlertsInDateRangeQuery(cameraMarkId, startDate, endDate), ct);
        return Ok(alerts);
    }

    [HttpPost("motion-alerts")]
    public async Task<ActionResult<Guid>> CreateMotionAlert(
        Guid geoMapId,
        Guid cameraMarkId,
        [FromBody] CreateMotionAlertRequest request,
        CancellationToken ct = default)
    {
        if (!Enum.TryParse<MotionSeverity>(request.Severity, true, out var severity))
            return BadRequest(new { error = $"Invalid severity: {request.Severity}" });

        var id = await Mediator.Send(new CreateCameraMotionAlertCommand(
            cameraMarkId,
            severity,
            request.MotionPercentage,
            request.SnapshotPath
        ), ct);
        return Ok(id);
    }

    [HttpPut("motion-alerts/{alertId:guid}/confirm")]
    public async Task<IActionResult> ConfirmMotionAlert(
        Guid geoMapId,
        Guid cameraMarkId,
        Guid alertId,
        CancellationToken ct = default)
    {
        await Mediator.Send(new ConfirmCameraMotionAlertCommand(alertId), ct);
        return NoContent();
    }

    [HttpPut("motion-alerts/{alertId:guid}/resolve")]
    public async Task<IActionResult> ResolveMotionAlert(
        Guid geoMapId,
        Guid cameraMarkId,
        Guid alertId,
        [FromBody] ResolveMotionAlertRequest request,
        CancellationToken ct = default)
    {
        await Mediator.Send(new ResolveCameraMotionAlertCommand(alertId, request.Notes), ct);
        return NoContent();
    }

    [HttpPut("motion-alerts/{alertId:guid}/link-video/{videoId:guid}")]
    public async Task<IActionResult> LinkMotionAlertToVideo(
        Guid geoMapId,
        Guid cameraMarkId,
        Guid alertId,
        Guid videoId,
        CancellationToken ct = default)
    {
        await Mediator.Send(new LinkMotionAlertToVideoCommand(alertId, videoId), ct);
        return NoContent();
    }

    [HttpDelete("motion-alerts/{alertId:guid}")]
    public async Task<IActionResult> DeleteMotionAlert(
        Guid geoMapId,
        Guid cameraMarkId,
        Guid alertId,
        CancellationToken ct = default)
    {
        await Mediator.Send(new DeleteCameraMotionAlertCommand(alertId), ct);
        return NoContent();
    }

    // ============== STATUS HISTORY ==============

    [HttpGet("status-history")]
    public async Task<ActionResult<List<CameraStatusHistoryListItemDto>>> GetStatusHistory(
        Guid geoMapId,
        Guid cameraMarkId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50,
        CancellationToken ct = default)
    {
        var statuses = await Mediator.Send(new GetCameraStatusHistoryQuery(cameraMarkId, skip, take), ct);
        return Ok(statuses);
    }

    [HttpGet("status-history/timeline")]
    public async Task<ActionResult<List<CameraStatusHistoryListItemDto>>> GetStatusHistoryTimeline(
        Guid geoMapId,
        Guid cameraMarkId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken ct = default)
    {
        var statuses = await Mediator.Send(
            new GetCameraStatusHistoryInDateRangeQuery(cameraMarkId, startDate, endDate), ct);
        return Ok(statuses);
    }

    [HttpGet("status-history/current")]
    public async Task<ActionResult<CameraStatusHistoryDto?>> GetCurrentStatus(
        Guid geoMapId,
        Guid cameraMarkId,
        CancellationToken ct = default)
    {
        var status = await Mediator.Send(new GetCameraCurrentStatusQuery(cameraMarkId), ct);
        return Ok(status);
    }

    [HttpPost("status-history")]
    public async Task<ActionResult<Guid>> CreateStatusHistory(
        Guid geoMapId,
        Guid cameraMarkId,
        [FromBody] CreateStatusHistoryRequest request,
        CancellationToken ct = default)
    {
        if (!Enum.TryParse<CameraStatusReason>(request.Reason, true, out var reason))
            return BadRequest(new { error = $"Invalid reason: {request.Reason}" });

        var id = await Mediator.Send(new CreateCameraStatusHistoryCommand(
            cameraMarkId,
            request.IsOnline,
            reason,
            request.Details,
            request.ResponseTimeMs
        ), ct);
        return Ok(id);
    }

    [HttpDelete("status-history/cleanup")]
    public async Task<IActionResult> CleanupOldStatusHistory(
        Guid geoMapId,
        Guid cameraMarkId,
        [FromQuery] int daysToKeep = 90,
        CancellationToken ct = default)
    {
        await Mediator.Send(new DeleteOldCameraStatusHistoryCommand(cameraMarkId, daysToKeep), ct);
        return NoContent();
    }
}

// ============== REQUEST MODELS ==============

public class CreateVideoArchiveRequest
{
    public string VideoPath { get; set; } = default!;
    public TimeSpan Duration { get; set; }
    public long FileSizeBytes { get; set; }
    public string Resolution { get; set; } = default!;
    public int FramesPerSecond { get; set; }
    public string? ThumbnailPath { get; set; }
}

public class CreateMotionAlertRequest
{
    public string Severity { get; set; } = default!;
    public double MotionPercentage { get; set; }
    public string? SnapshotPath { get; set; }
}

public class ResolveMotionAlertRequest
{
    public string? Notes { get; set; }
}

public class CreateStatusHistoryRequest
{
    public bool IsOnline { get; set; }
    public string Reason { get; set; } = default!;
    public string? Details { get; set; }
    public int? ResponseTimeMs { get; set; }
}

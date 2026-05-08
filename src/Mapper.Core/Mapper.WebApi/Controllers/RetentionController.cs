using Asp.Versioning;
using Mapper.Application.Features.Retention.Commands;
using Mapper.Application.Features.Retention.DTOs;
using Mapper.Application.Features.Retention.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Mapper.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/retention")]
public class RetentionController : BaseController
{
    [HttpGet("archive/preview")]
    public async Task<ActionResult<ArchiveRetentionPreviewDto>> GetArchiveRetentionPreview(
        [FromQuery] Guid? cameraMarkId,
        [FromQuery] DateTimeOffset? now,
        [FromQuery] int motionVideoRetentionDays = 90,
        [FromQuery] int noMotionVideoRetentionDays = 7,
        [FromQuery] int archivedVideoRetentionDays = 365,
        [FromQuery] int take = 100,
        CancellationToken ct = default)
    {
        var preview = await Mediator.Send(
            new GetArchiveRetentionPreviewQuery(
                cameraMarkId,
                now,
                motionVideoRetentionDays,
                noMotionVideoRetentionDays,
                archivedVideoRetentionDays,
                take),
            ct);

        return Ok(preview);
    }

    [HttpPost("archive/cleanup")]
    public async Task<ActionResult<ArchiveRetentionCleanupResultDto>> CleanupArchiveRetention(
        [FromBody] ArchiveRetentionCleanupRequest request,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(
            new CleanupArchiveRetentionCommand(
                request.CameraMarkId,
                request.Now,
                request.MotionVideoRetentionDays,
                request.NoMotionVideoRetentionDays,
                request.ArchivedVideoRetentionDays,
                request.Take,
                request.DryRun,
                request.Confirm),
            ct);

        return Ok(result);
    }
}

public class ArchiveRetentionCleanupRequest
{
    public Guid? CameraMarkId { get; set; }
    public DateTimeOffset? Now { get; set; }
    public int MotionVideoRetentionDays { get; set; } = 90;
    public int NoMotionVideoRetentionDays { get; set; } = 7;
    public int ArchivedVideoRetentionDays { get; set; } = 365;
    public int Take { get; set; } = 100;
    public bool DryRun { get; set; } = true;
    public bool Confirm { get; set; }
}

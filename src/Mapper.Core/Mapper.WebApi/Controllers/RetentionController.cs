using Asp.Versioning;
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
}

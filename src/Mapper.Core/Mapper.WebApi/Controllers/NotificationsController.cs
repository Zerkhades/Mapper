using Asp.Versioning;
using Mapper.Application.Features.Notifications.DTOs;
using Mapper.Application.Features.Notifications.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Mapper.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/notifications")]
public class NotificationsController : BaseController
{
    [HttpGet("smart")]
    public async Task<ActionResult<List<SmartNotificationDto>>> GetSmartNotifications(
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to,
        [FromQuery] int offlineGraceMinutes = 5,
        [FromQuery] int top = 50,
        CancellationToken ct = default)
    {
        var notifications = await Mediator.Send(
            new GetSmartNotificationsQuery(from, to, offlineGraceMinutes, top),
            ct);

        return Ok(notifications);
    }
}

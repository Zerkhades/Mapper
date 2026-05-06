using Asp.Versioning;
using Mapper.Application.Features.Analytics.DTOs;
using Mapper.Application.Features.Analytics.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Mapper.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/dashboard")]
public class DashboardController : BaseController
{
    [HttpGet("operator")]
    public async Task<ActionResult<OperatorDashboardDto>> GetOperatorDashboard(
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to,
        [FromQuery] int top = 5,
        CancellationToken ct = default)
    {
        var dashboard = await Mediator.Send(new GetOperatorDashboardQuery(from, to, top), ct);
        return Ok(dashboard);
    }
}

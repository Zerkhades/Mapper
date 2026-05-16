using Asp.Versioning;
using Mapper.Application.Features.Routes.Commands;
using Mapper.Application.Features.Routes.DTOs;
using Mapper.Application.Features.Routes.Queries;
using Mapper.WebApi.Models.Routes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mapper.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/geomaps/{geoMapId:guid}/routes")]
public class RoutesController : BaseController
{
    private readonly IMediator _mediator;

    public RoutesController(IMediator mediator) => _mediator = mediator;

    [HttpGet("nodes")]
    public async Task<ActionResult<IReadOnlyList<RouteNodeDto>>> GetNodes(Guid geoMapId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetRouteNodesQuery(geoMapId), ct));

    [HttpPost("nodes")]
    public async Task<ActionResult<Guid>> CreateNode(
        Guid geoMapId,
        [FromBody] CreateRouteNodeRequest request,
        CancellationToken ct)
        => Ok(await _mediator.Send(new CreateRouteNodeCommand(
            geoMapId,
            request.X,
            request.Y,
            request.Title,
            request.GeoMarkId), ct));

    [HttpPut("nodes/{routeNodeId:guid}")]
    public async Task<IActionResult> UpdateNode(
        Guid geoMapId,
        Guid routeNodeId,
        [FromBody] UpdateRouteNodeRequest request,
        CancellationToken ct)
    {
        await _mediator.Send(new UpdateRouteNodeCommand(
            geoMapId,
            routeNodeId,
            request.X,
            request.Y,
            request.Title,
            request.GeoMarkId), ct);

        return NoContent();
    }

    [HttpDelete("nodes/{routeNodeId:guid}")]
    public async Task<IActionResult> DeleteNode(Guid geoMapId, Guid routeNodeId, CancellationToken ct)
    {
        await _mediator.Send(new DeleteRouteNodeCommand(geoMapId, routeNodeId), ct);
        return NoContent();
    }

    [HttpGet("edges")]
    public async Task<ActionResult<IReadOnlyList<RouteEdgeDto>>> GetEdges(Guid geoMapId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetRouteEdgesQuery(geoMapId), ct));

    [HttpPost("edges")]
    public async Task<ActionResult<Guid>> CreateEdge(
        Guid geoMapId,
        [FromBody] CreateRouteEdgeRequest request,
        CancellationToken ct)
        => Ok(await _mediator.Send(new CreateRouteEdgeCommand(
            geoMapId,
            request.FromNodeId,
            request.ToNodeId,
            request.IsBidirectional,
            request.CostOverride,
            request.Description), ct));

    [HttpPut("edges/{routeEdgeId:guid}")]
    public async Task<IActionResult> UpdateEdge(
        Guid geoMapId,
        Guid routeEdgeId,
        [FromBody] UpdateRouteEdgeRequest request,
        CancellationToken ct)
    {
        await _mediator.Send(new UpdateRouteEdgeCommand(
            geoMapId,
            routeEdgeId,
            request.IsBidirectional,
            request.CostOverride,
            request.IsDisabled,
            request.Description), ct);

        return NoContent();
    }

    [HttpDelete("edges/{routeEdgeId:guid}")]
    public async Task<IActionResult> DeleteEdge(Guid geoMapId, Guid routeEdgeId, CancellationToken ct)
    {
        await _mediator.Send(new DeleteRouteEdgeCommand(geoMapId, routeEdgeId), ct);
        return NoContent();
    }

    [HttpPost("calculate")]
    public async Task<ActionResult<CalculatedRouteDto>> Calculate(
        Guid geoMapId,
        [FromBody] CalculateRouteRequest request,
        CancellationToken ct)
        => Ok(await _mediator.Send(new CalculateRouteQuery(
            geoMapId,
            request.Start,
            request.End,
            request.WalkingSpeed), ct));
}

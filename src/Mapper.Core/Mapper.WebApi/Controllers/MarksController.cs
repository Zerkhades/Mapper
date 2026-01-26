using Asp.Versioning;
using Mapper.Application.Features.DTOs;
using Mapper.Application.Features.GeoMarks.Commands;
using Mapper.Application.Features.GeoMarks.Commands.CameraMarkCommands;
using Mapper.Application.Features.GeoMarks.Commands.DeleteGeoMark;
using Mapper.Application.Features.GeoMarks.Commands.TransitionMarkCommands;
using Mapper.Application.Features.GeoMarks.Commands.WorkplaceMarkCommands;
using Mapper.Application.Features.GeoMarks.Queries;
using Mapper.Domain;
using Mapper.WebApi.Models.Marks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mapper.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/geomaps/{geoMapId:guid}/marks")]
public class MarksController : BaseController
{
    private readonly IMediator _mediator;

    public MarksController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GeoMarkDto>>> GetAll(Guid geoMapId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetGeoMarksQuery(geoMapId, null), ct));

    [HttpGet("{geoMarkId:guid}")]
    public async Task<ActionResult<GeoMarkDto>> GetById(Guid geoMapId, Guid geoMarkId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetGeoMarkByIdQuery(geoMapId, geoMarkId), ct));

    [HttpGet("transition")]
    public async Task<ActionResult<IReadOnlyList<GeoMarkDto>>> GetTransitions(Guid geoMapId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetGeoMarksQuery(geoMapId, GeoMarkType.Transition), ct));

    [HttpGet("workplace")]
    public async Task<ActionResult<IReadOnlyList<GeoMarkDto>>> GetWorkplaces(Guid geoMapId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetGeoMarksQuery(geoMapId, GeoMarkType.Workplace), ct));

    [HttpGet("camera")]
    public async Task<ActionResult<IReadOnlyList<GeoMarkDto>>> GetCameras(Guid geoMapId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetGeoMarksQuery(geoMapId, GeoMarkType.Camera), ct));

    [HttpPost("transition")]
    public async Task<ActionResult<Guid>> AddTransitionMark(Guid geoMapId, [FromBody] CreateTransitionMarkRequest req, CancellationToken ct)
        => Ok(await _mediator.Send(new AddTransitionMarkCommand(
            geoMapId, req.X, req.Y, req.Title, req.Description, req.TargetGeoMapId
        ), ct));

    [HttpPut("transition/{geoMarkId:guid}")]
    public async Task<IActionResult> UpdateTransitionMark(Guid geoMapId, Guid geoMarkId, [FromBody] CreateTransitionMarkRequest req, CancellationToken ct)
    {
        await _mediator.Send(new UpdateTransitionMarkCommand(
            geoMapId,
            geoMarkId,
            req.X,
            req.Y,
            req.Title,
            req.Description,
            req.TargetGeoMapId
        ), ct);
        return NoContent();
    }

    [HttpPost("workplace")]
    public async Task<ActionResult<Guid>> AddWorkplaceMark(Guid geoMapId, [FromBody] CreateWorkplaceMarkRequest req, CancellationToken ct)
        => Ok(await _mediator.Send(new AddWorkplaceMarkCommand(
            geoMapId, req.X, req.Y, req.Title, req.Description, req.WorkplaceCode, req.EmployeeIds
        ), ct));

    [HttpPut("workplace/{geoMarkId:guid}")]
    public async Task<IActionResult> UpdateWorkplaceMark(Guid geoMapId, Guid geoMarkId, [FromBody] CreateWorkplaceMarkRequest req, CancellationToken ct)
    {
        await _mediator.Send(new UpdateWorkplaceMarkCommand(
            geoMapId,
            geoMarkId,
            req.X,
            req.Y,
            req.Title,
            req.Description,
            req.WorkplaceCode,
            req.EmployeeIds
        ), ct);
        return NoContent();
    }

    [HttpPost("camera")]
    public async Task<ActionResult<Guid>> AddCameraMark(Guid geoMapId, [FromBody] CreateCameraMarkRequest req, CancellationToken ct)
        => Ok(await _mediator.Send(new AddCameraMarkCommand(
            geoMapId, req.X, req.Y, req.Title, req.Description, req.CameraName, req.StreamUrl
        ), ct));

    [HttpPut("camera/{geoMarkId:guid}")]
    public async Task<IActionResult> UpdateCameraMark(Guid geoMapId, Guid geoMarkId, [FromBody] CreateCameraMarkRequest req, CancellationToken ct)
    {
        await _mediator.Send(new UpdateCameraMarkCommand(
            geoMapId,
            geoMarkId,
            req.X,
            req.Y,
            req.Title,
            req.Description,
            req.CameraName,
            req.StreamUrl
        ), ct);
        return NoContent();
    }

    [HttpDelete("{geoMarkId:guid}")]
    public async Task<IActionResult> Delete(Guid geoMapId, Guid geoMarkId, CancellationToken ct)
    {
        await _mediator.Send(new DeleteGeoMarkCommand(geoMapId, geoMarkId), ct);
        return NoContent();
    }
}


using Asp.Versioning;
using Mapper.Application.Features.DTOs;
using Mapper.Application.Features.GeoMarks.Commands;
using Mapper.Application.Features.GeoMarks.Commands.CameraMarkCommands;
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

    [HttpPost("workplace")]
    public async Task<ActionResult<Guid>> AddWorkplaceMark(Guid geoMapId, [FromBody] CreateWorkplaceMarkRequest req, CancellationToken ct)
        => Ok(await _mediator.Send(new AddWorkplaceMarkCommand(
            geoMapId, req.X, req.Y, req.Title, req.Description, req.WorkplaceCode, req.EmployeeIds
        ), ct));

    [HttpPost("camera")]
    public async Task<ActionResult<Guid>> AddCameraMark(Guid geoMapId, [FromBody] CreateCameraMarkRequest req, CancellationToken ct)
        => Ok(await _mediator.Send(new AddCameraMarkCommand(
            geoMapId, req.X, req.Y, req.Title, req.Description, req.CameraName, req.StreamUrl
        ), ct));
}


using Asp.Versioning;
using Mapper.Application.Features.GeoMarks.Commands;
using Mapper.Application.Features.GeoMarks.Commands.AddCameraMark;
using Mapper.Application.Features.GeoMarks.Commands.AddTransitionMark;
using Mapper.Application.Features.GeoMarks.Commands.AddWorkplaceMark;
using Mapper.WebApi.Models; // где DTO запросов
using Mapper.WebApi.Models.Marks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mapper.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/geomaps")]
public class MarksController : ControllerBase
{
    private readonly IMediator _mediator;

    public MarksController(IMediator mediator) => _mediator = mediator;

    //[Authorize]
    [HttpPost("{geoMapId:guid}/marks/transition")]
    public async Task<ActionResult<Guid>> AddTransitionMark(Guid geoMapId, [FromBody] CreateTransitionMarkRequest req, CancellationToken ct)
    {
        var id = await _mediator.Send(new AddTransitionMarkCommand(
            geoMapId, req.X, req.Y, req.Title, req.Description, req.TargetGeoMapId
        ), ct);

        return Ok(id);
    }

    //[Authorize]
    [HttpPost("{geoMapId:guid}/marks/workplace")]
    public async Task<ActionResult<Guid>> AddWorkplaceMark(Guid geoMapId, [FromBody] CreateWorkplaceMarkRequest req, CancellationToken ct)
    {
        var id = await _mediator.Send(new AddWorkplaceMarkCommand(
            geoMapId, req.X, req.Y, req.Title, req.Description, req.WorkplaceCode, req.EmployeeIds
        ), ct);

        return Ok(id);
    }

    //[Authorize]
    [HttpPost("{geoMapId:guid}/marks/camera")]
    public async Task<ActionResult<Guid>> AddCameraMark(Guid geoMapId, [FromBody] CreateCameraMarkRequest req, CancellationToken ct)
    {
        var id = await _mediator.Send(new AddCameraMarkCommand(
            geoMapId, req.X, req.Y, req.Title, req.Description, req.CameraName, req.StreamUrl
        ), ct);

        return Ok(id);
    }
}

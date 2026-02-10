using Asp.Versioning;
using Mapper.Application.Features.Employees.Commands.CreateEmployee;
using Mapper.Application.Features.Employees.Commands.DeleteEmployee;
using Mapper.Application.Features.Employees.Commands.UpdateEmployee;
using Mapper.Application.Features.Employees.Queries.GetEmployeeDetails;
using Mapper.Application.Features.Employees.Queries.GetEmployeeList;
using Mapper.WebApi.Models.Employees;
using Microsoft.AspNetCore.Mvc;

namespace Mapper.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
[Route("api/v{version:apiVersion}/employees")]
public class EmployeesController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateEmployeeRequest request, CancellationToken ct)
        => Ok(await Mediator.Send(new CreateEmployeeCommand(
            request.FirstName,
            request.Surname,
            request.Patronymic,
            request.Phone,
            request.Email,
            request.Cabinet,
            request.Comment,
            request.GeoMarkId
        ), ct));

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployeeListItemDto>>> GetList([FromQuery] Guid? geoMarkId, CancellationToken ct)
        => Ok(await Mediator.Send(new GetEmployeeListQuery(geoMarkId), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeDetailsDto>> Get(Guid id, CancellationToken ct)
        => Ok(await Mediator.Send(new GetEmployeeDetailsQuery(id), ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeRequest request, CancellationToken ct)
    {
        await Mediator.Send(new UpdateEmployeeCommand(
            id,
            request.FirstName,
            request.Surname,
            request.Patronymic,
            request.Phone,
            request.Email,
            request.Cabinet,
            request.Comment,
            request.GeoMarkId
        ), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await Mediator.Send(new DeleteEmployeeCommand(id), ct);
        return NoContent();
    }
}

using FluentValidation;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.Employees.Commands.DeleteEmployee;

public record DeleteEmployeeCommand(Guid Id) : IRequest;

public class DeleteEmployeeValidator : AbstractValidator<DeleteEmployeeCommand>
{
    public DeleteEmployeeValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class DeleteEmployeeHandler : IRequestHandler<DeleteEmployeeCommand>
{
    private readonly IMapperDbContext _db;

    public DeleteEmployeeHandler(IMapperDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DeleteEmployeeCommand request, CancellationToken ct)
    {
        var employee = await _db.Employees
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (employee is null)
            throw new NotFoundException($"Employee {request.Id} not found", request.Id);

        // Мягкое удаление через архивацию
        employee.IsArchived = true;

        await _db.SaveChangesAsync(ct);
    }
}

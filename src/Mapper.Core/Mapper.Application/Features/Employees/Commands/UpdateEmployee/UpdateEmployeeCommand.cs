using FluentValidation;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.Employees.Commands.UpdateEmployee;

public record UpdateEmployeeCommand(
    Guid Id,
    string FirstName,
    string Surname,
    string? Patronymic,
    string? Phone,
    string? Email,
    string? Cabinet,
    string? Comment,
    Guid GeoMarkId
) : IRequest;

public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Surname).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Patronymic).MaximumLength(100);
        RuleFor(x => x.Phone).MaximumLength(20);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Cabinet).MaximumLength(50);
        RuleFor(x => x.Comment).MaximumLength(500);
        RuleFor(x => x.GeoMarkId).NotEmpty();
    }
}

public class UpdateEmployeeHandler : IRequestHandler<UpdateEmployeeCommand>
{
    private readonly IMapperDbContext _db;

    public UpdateEmployeeHandler(IMapperDbContext db)
    {
        _db = db;
    }

    public async Task Handle(UpdateEmployeeCommand request, CancellationToken ct)
    {
        var employee = await _db.Employees
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (employee is null)
            throw new NotFoundException($"Employee {request.Id} not found", request.Id);

        // Проверяем, что новый GeoMark существует и это WorkplaceMark
        var geoMark = await _db.GeoMarks
            .OfType<WorkplaceMark>()
            .FirstOrDefaultAsync(x => x.Id == request.GeoMarkId, ct);

        if (geoMark is null)
            throw new NotFoundException($"WorkplaceMark {request.GeoMarkId} not found", request.GeoMarkId);

        employee.FirstName = request.FirstName;
        employee.Surname = request.Surname;
        employee.Patronymic = request.Patronymic;
        employee.Phone = request.Phone;
        employee.Email = request.Email;
        employee.Cabinet = request.Cabinet;
        employee.Comment = request.Comment;
        employee.GeoMarkId = request.GeoMarkId;

        await _db.SaveChangesAsync(ct);
    }
}

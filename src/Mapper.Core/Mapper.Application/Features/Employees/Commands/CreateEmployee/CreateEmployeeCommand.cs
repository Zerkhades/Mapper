using FluentValidation;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.Employees.Commands.CreateEmployee;

public record CreateEmployeeCommand(
    string FirstName,
    string Surname,
    string? Patronymic,
    string? Phone,
    string? Email,
    string? Cabinet,
    string? Comment,
    Guid GeoMarkId
) : IRequest<Guid>;

public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeValidator()
    {
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

public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, Guid>
{
    private readonly IMapperDbContext _db;

    public CreateEmployeeHandler(IMapperDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> Handle(CreateEmployeeCommand request, CancellationToken ct)
    {
        // Проверяем, что GeoMark существует и это WorkplaceMark
        var geoMark = await _db.GeoMarks
            .OfType<WorkplaceMark>()
            .FirstOrDefaultAsync(x => x.Id == request.GeoMarkId, ct);

        if (geoMark is null)
            throw new NotFoundException($"WorkplaceMark {request.GeoMarkId} not found", request.GeoMarkId);

        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            Surname = request.Surname,
            Patronymic = request.Patronymic,
            Phone = request.Phone,
            Email = request.Email,
            Cabinet = request.Cabinet,
            Comment = request.Comment,
            GeoMarkId = request.GeoMarkId
        };

        _db.Employees.Add(employee);
        await _db.SaveChangesAsync(ct);

        return employee.Id;
    }
}

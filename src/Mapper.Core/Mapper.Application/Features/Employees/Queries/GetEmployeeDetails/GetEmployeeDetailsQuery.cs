using AutoMapper;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.Employees.Queries.GetEmployeeDetails;

public record EmployeeDetailsDto(
    Guid Id,
    string FirstName,
    string Surname,
    string? Patronymic,
    string FullName,
    string? Phone,
    string? Email,
    string? Cabinet,
    string? Comment,
    Guid GeoMarkId
);

public record GetEmployeeDetailsQuery(Guid Id) : IRequest<EmployeeDetailsDto>;

public class GetEmployeeDetailsHandler : IRequestHandler<GetEmployeeDetailsQuery, EmployeeDetailsDto>
{
    private readonly IMapperDbContext _db;
    private readonly IMapper _mapper;

    public GetEmployeeDetailsHandler(IMapperDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<EmployeeDetailsDto> Handle(GetEmployeeDetailsQuery request, CancellationToken ct)
    {
        var employee = await _db.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == request.Id && !e.IsArchived, ct);

        if (employee is null)
            throw new NotFoundException($"Employee {request.Id} not found", request.Id);

        return _mapper.Map<EmployeeDetailsDto>(employee);
    }
}

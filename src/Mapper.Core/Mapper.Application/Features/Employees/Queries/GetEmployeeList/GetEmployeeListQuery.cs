using AutoMapper;
using Mapper.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.Employees.Queries.GetEmployeeList;

public record EmployeeListItemDto(
    Guid Id,
    string FirstName,
    string Surname,
    string? Patronymic,
    string FullName,
    string? Phone,
    string? Email,
    string? Cabinet,
    Guid GeoMarkId
);

public record GetEmployeeListQuery(Guid? GeoMarkId = null) : IRequest<IReadOnlyList<EmployeeListItemDto>>;

public class GetEmployeeListHandler : IRequestHandler<GetEmployeeListQuery, IReadOnlyList<EmployeeListItemDto>>
{
    private readonly IMapperDbContext _db;
    private readonly IMapper _mapper;

    public GetEmployeeListHandler(IMapperDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<EmployeeListItemDto>> Handle(GetEmployeeListQuery request, CancellationToken ct)
    {
        var query = _db.Employees
            .AsNoTracking()
            .Where(e => !e.IsArchived);

        if (request.GeoMarkId.HasValue)
        {
            query = query.Where(e => e.GeoMarkId == request.GeoMarkId.Value);
        }

        return await query
            .OrderBy(e => e.Surname)
            .ThenBy(e => e.FirstName)
            .Select(e => _mapper.Map<EmployeeListItemDto>(e))
            .ToListAsync(ct);

    }
}

using AutoMapper;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Features.DTOs;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.GeoMarks.Queries;

public record GetGeoMarkByIdQuery(Guid GeoMapId, Guid GeoMarkId) : IRequest<GeoMarkDto>;

public class GetGeoMarkByIdHandler : IRequestHandler<GetGeoMarkByIdQuery, GeoMarkDto>
{
    private readonly IMapperDbContext _db;
    private readonly IMapper _mapper;

    public GetGeoMarkByIdHandler(IMapperDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<GeoMarkDto> Handle(GetGeoMarkByIdQuery request, CancellationToken ct)
    {
        var mapExists = await _db.GeoMaps.AnyAsync(x => x.Id == request.GeoMapId, ct);
        if (!mapExists)
            throw new NotFoundException($"GeoMap {request.GeoMapId} not found", request.GeoMapId);

        var mark = await _db.GeoMarks
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.GeoMarkId && x.GeoMapId == request.GeoMapId, ct);

        if (mark is null)
            throw new NotFoundException($"GeoMark {request.GeoMarkId} not found", request.GeoMarkId);

        var dto = _mapper.Map<GeoMarkDto>(mark);

        if (mark is WorkplaceMark)
        {
            var empIds = await _db.Employees
                .AsNoTracking()
                .Where(e => e.GeoMarkId == mark.Id)
                .Select(e => e.Id)
                .ToListAsync(ct);
            dto = dto with { EmployeeIds = empIds };
        }

        return dto;
    }
}

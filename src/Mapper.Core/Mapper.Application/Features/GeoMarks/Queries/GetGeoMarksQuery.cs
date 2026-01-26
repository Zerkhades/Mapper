using AutoMapper;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Features;
using Mapper.Application.Features.DTOs;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.GeoMarks.Queries;

public record GetGeoMarksQuery(Guid GeoMapId, GeoMarkType? Type = null) : IRequest<IReadOnlyList<GeoMarkDto>>;

public class GetGeoMarksHandler : IRequestHandler<GetGeoMarksQuery, IReadOnlyList<GeoMarkDto>>
{
    private readonly IMapperDbContext _db;
    private readonly IMapper _mapper;

    public GetGeoMarksHandler(IMapperDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<GeoMarkDto>> Handle(GetGeoMarksQuery request, CancellationToken ct)
    {
        var mapExists = await _db.GeoMaps.AnyAsync(x => x.Id == request.GeoMapId, ct);
        if (!mapExists)
            throw new NotFoundException($"GeoMap {request.GeoMapId} not found", request.GeoMapId);

        var q = _db.GeoMarks.AsNoTracking().Where(m => m.GeoMapId == request.GeoMapId);

        if (request.Type is not null)
            q = q.Where(m => m.Type == request.Type);

        var marks = await q.ToListAsync(ct);

        var workplaceIds = marks
            .OfType<WorkplaceMark>()
            .Select(w => w.Id)
            .ToList();

        Dictionary<Guid, List<Guid>> employeesByWorkplace = new();
        if (workplaceIds.Count > 0)
        {
            var rels = await _db.Employees
                .AsNoTracking()
                .Where(x => workplaceIds.Contains(x.GeoMarkId))
                .ToListAsync(ct);

            employeesByWorkplace = rels
                .GroupBy(x => x.GeoMarkId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Id).ToList());
        }

        var dto = marks.Select(m =>
        {
            var d = _mapper.Map<GeoMarkDto>(m);

            if (m is WorkplaceMark wm && employeesByWorkplace.TryGetValue(wm.Id, out var empIds))
            {
                d = d with { EmployeeIds = empIds };
            }

            return d;
        }).ToList();

        return dto;
    }
}

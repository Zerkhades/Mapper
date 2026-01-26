using AutoMapper;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Features.DTOs;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.GeoMaps.Queries.GetGeoMapById
{
    public record GetGeoMapByIdQuery(Guid Id) : IRequest<GeoMapDetailsDto>;

    public class GetGeoMapByIdHandler : IRequestHandler<GetGeoMapByIdQuery, GeoMapDetailsDto>
    {
        private readonly IMapperDbContext _db;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public GetGeoMapByIdHandler(IMapperDbContext db, IMapper mapper, ICacheService cacheService)
        {
            _db = db;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<GeoMapDetailsDto> Handle(GetGeoMapByIdQuery request, CancellationToken ct)
        {
            var cacheKey = $"geomap:{request.Id}";

            var cached = await _cacheService.GetAsync<GeoMapDetailsDto>(cacheKey, ct);
            if (cached is not null) return cached;

            var map = await _db.GeoMaps
                .AsNoTracking()
                .Include(x => x.Marks)
                .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

            if (map is null)
                throw new NotFoundException($"GeoMap {request.Id} not found", request.Id);

            var workplaceIds = map.Marks
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

            var marks = map.Marks.Select(m =>
            {
                var d = _mapper.Map<GeoMarkDto>(m);

                if (m is WorkplaceMark wm && employeesByWorkplace.TryGetValue(wm.Id, out var empIds))
                {
                    d = d with { EmployeeIds = empIds };
                }

                return d;
            }).ToList();

            var dto = _mapper.Map<GeoMapDetailsDto>(map);
            dto = dto with { Marks = marks };

            var imageKey = map.ImagePath;
            dto = dto with
            {
                ImageUrl = $"/api/files/{imageKey}"
            };

            await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromSeconds(30), ct);
            return dto;
        }
    }
}

using AutoMapper;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.GeoMaps
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

            var dto = _mapper.Map<GeoMapDetailsDto>(map);

            // ВАЖНО: map.ImagePath (или как поле называется в домене) — это S3 key, НЕ URL.
            // Поэтому фронту отдаём URL на WebApi, который стримит файл из S3:
            // GET /api/files/{key}
            var imageKey = map.ImagePath; // <-- если у тебя свойство называется иначе, поправь здесь

            dto = dto with
            {
                ImageUrl = $"/api/files/{imageKey}"
            };

            await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromSeconds(30), ct);
            return dto;
        }
    }
}

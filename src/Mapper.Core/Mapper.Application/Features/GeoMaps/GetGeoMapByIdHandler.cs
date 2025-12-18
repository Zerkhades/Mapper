using AutoMapper;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            _db = db; _mapper = mapper; _cacheService = cacheService;
        }

        public async Task<GeoMapDetailsDto> Handle(GetGeoMapByIdQuery request, CancellationToken ct)
        {
            // do not forget to cache the result after getting from db
            // also add removeasync while changing marks
            var cache = await _cacheService.GetAsync<GeoMapDetailsDto>(request.Id.ToString(), ct);
            if (cache is not null) return cache;
            var map = await _db.GeoMaps
                .AsNoTracking()
                .Include(x => x.Marks)
                .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

            if (map is null) throw new NotFoundException($"GeoMap {request.Id} not found", request.Id);

            // Проще всего маппить руками/профилем (ниже)
            return _mapper.Map<GeoMapDetailsDto>(map);
        }
    }

}

using AutoMapper;
using Mapper.Application.Features.DTOs;
using Mapper.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.GeoMaps.Queries.GetGeoMapList
{
    public record GetGeoMapListQuery : IRequest<IReadOnlyList<GeoMapListItemDto>>;

    public class GetGeoMapListHandler : IRequestHandler<GetGeoMapListQuery, IReadOnlyList<GeoMapListItemDto>>
    {
        private readonly IMapperDbContext _db;
        private readonly IMapper _mapper;

        public GetGeoMapListHandler(IMapperDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<GeoMapListItemDto>> Handle(GetGeoMapListQuery request, CancellationToken ct)
        {
            var maps = await _db.GeoMaps
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync(ct);

            return maps.Select(_mapper.Map<GeoMapListItemDto>).ToList();
        }
    }
}

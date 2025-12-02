using AutoMapper;
using AutoMapper.QueryableExtensions;
using Mapper.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.CommandsAndQueries.GeoMark.Queries.GetGeoMarkList
{
    public class GetGeoMarkListQueryHandler
            : IRequestHandler<GetGeoMarkListQuery, GeoMarkListVm>
    {
        private readonly IMapperDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetGeoMarkListQueryHandler(IMapperDbContext dbContext,
            IMapper mapper) =>
            (_dbContext, _mapper) = (dbContext, mapper);

        public async Task<GeoMarkListVm> Handle(GetGeoMarkListQuery request, CancellationToken cancellationToken)
        {
            var geoMarksQuery = await _dbContext.GeoMarks
                .ProjectTo<GeoMarkLookupDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new GeoMarkListVm { GeoMarks = geoMarksQuery };
        }
    }
}

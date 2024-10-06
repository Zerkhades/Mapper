using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Mapper.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Queries.GetGeoMapList
{
    public class GetNoteListQueryHandler
        : IRequestHandler<GetGeoMapListQuery, GeoMapListVm>
    {
        private readonly IMapperDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetNoteListQueryHandler(IMapperDbContext dbContext,
            IMapper mapper) =>
            (_dbContext, _mapper) = (dbContext, mapper);

        public async Task<GeoMapListVm> Handle(GetGeoMapListQuery request, CancellationToken cancellationToken)
        {
            //var geoMapsQuery = await _dbContext.Notes
            //    .Where(note => note.UserId == request.UserId)
            //    .ProjectTo<GeoMapLookupDto>(_mapper.ConfigurationProvider)
            //    .ToListAsync(cancellationToken);

            var geoMapsQuery = await _dbContext.GeoMaps
                .Where(geomap => geomap.IsArchived != false)
                .ProjectTo<GeoMapLookupDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new GeoMapListVm { GeoMaps = geoMapsQuery };
        }

    }
}

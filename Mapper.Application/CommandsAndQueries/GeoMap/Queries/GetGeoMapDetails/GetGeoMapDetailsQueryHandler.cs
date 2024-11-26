using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Queries.GetGeoMapDetails
{
    public class GetGeoMapDetailsQueryHandler : IRequestHandler<GetGeoMapDetailsQuery, GeoMapDetailsVm>
    {
        private readonly IMapperDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetGeoMapDetailsQueryHandler(IMapperDbContext dbContext,
            IMapper mapper) => (_dbContext, _mapper) = (dbContext, mapper);

        public async Task<GeoMapDetailsVm> Handle(GetGeoMapDetailsQuery request,
            CancellationToken cancellationToken)
        {
            var entity = await _dbContext.GeoMaps
                .FirstOrDefaultAsync(geoMap =>
                    geoMap.Id == request.Id, cancellationToken);

            if (entity == null || entity.Id != request.Id)
            {
                throw new NotFoundException(nameof(Domain.GeoMap), request.Id);
            }

            return _mapper.Map<GeoMapDetailsVm>(entity);
        }
    }
}

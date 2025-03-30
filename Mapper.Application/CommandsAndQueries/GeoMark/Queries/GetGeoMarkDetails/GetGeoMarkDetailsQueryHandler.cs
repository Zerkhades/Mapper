using AutoMapper;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.CommandsAndQueries.GeoMark.Queries.GetGeoMarkDetails
{
    public class GetGeoMarkDetailsQueryHandler : IRequestHandler<GetGeoMarkDetailsQuery, GeoMarkDetailsVm>
    {
        private readonly IMapperDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetGeoMarkDetailsQueryHandler(IMapperDbContext dbContext, IMapper mapper) =>
            (_dbContext, _mapper) = (dbContext, mapper);

        public async Task<GeoMarkDetailsVm> Handle(GetGeoMarkDetailsQuery request, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.GeoMarks
                .FirstOrDefaultAsync(geoMark => geoMark.Id == request.Id, cancellationToken);

            if (entity == null || entity.Id != request.Id)
            {
                throw new NotFoundException(nameof(Domain.GeoMark), request.Id);
            }

            return _mapper.Map<GeoMarkDetailsVm>(entity);
        }
    }
}

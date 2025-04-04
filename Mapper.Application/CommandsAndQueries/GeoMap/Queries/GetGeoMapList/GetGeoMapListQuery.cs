using MediatR;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Queries.GetGeoMapList
{
    public class GetGeoMapListQuery : IRequest<GeoMapListVm>
    {
        public Guid UserId { get; set; }
    }
}

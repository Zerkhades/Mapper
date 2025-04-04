using MediatR;


namespace Mapper.Application.CommandsAndQueries.GeoMark.Queries.GetGeoMarkList
{
    public class GetGeoMarkListQuery: IRequest<GeoMarkListVm>
    {
        public Guid UserId { get; set; }
    }
}

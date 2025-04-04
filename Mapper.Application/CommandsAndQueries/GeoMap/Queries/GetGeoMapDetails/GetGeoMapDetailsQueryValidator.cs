using FluentValidation;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Queries.GetGeoMapDetails
{
    public class GetGeoMapDetailsQueryValidator : AbstractValidator<GetGeoMapDetailsQuery>
    {
        public GetGeoMapDetailsQueryValidator() 
        {
            RuleFor(geoMap => geoMap.Id).NotEqual(Guid.Empty);
        }
    }
}

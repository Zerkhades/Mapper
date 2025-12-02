using FluentValidation;

namespace Mapper.Application.CommandsAndQueries.GeoMark.Queries.GetGeoMarkDetails
{
    public class GetGeoMarkDetailsQueryValidator : AbstractValidator<GetGeoMarkDetailsQuery>
    {
        public GetGeoMarkDetailsQueryValidator()
        {
            RuleFor(geoMark => geoMark.Id).NotEqual(Guid.Empty);
        }
    }
}

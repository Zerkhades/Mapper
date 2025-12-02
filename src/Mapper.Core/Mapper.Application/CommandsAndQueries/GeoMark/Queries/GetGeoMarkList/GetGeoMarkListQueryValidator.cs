using FluentValidation;

namespace Mapper.Application.CommandsAndQueries.GeoMark.Queries.GetGeoMarkList
{
    public class GetGeoMarkListQueryValidator : AbstractValidator<GetGeoMarkListQuery>
    {
        public GetGeoMarkListQueryValidator()
        {
            RuleFor(x => x.UserId).NotEqual(Guid.Empty);
        }
    }
}

using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

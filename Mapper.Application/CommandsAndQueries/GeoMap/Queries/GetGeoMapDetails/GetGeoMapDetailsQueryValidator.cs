using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Queries.GetGeoMapDetails
{
    internal class GetGeoMapDetailsQueryValidator : AbstractValidator<GetGeoMapDetailsQuery>
    {
        public GetGeoMapDetailsQueryValidator() 
        {
            RuleFor(note => note.Id).NotEqual(Guid.Empty);
        }
    }
}

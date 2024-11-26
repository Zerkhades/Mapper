using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Queries.GetGeoMapDetails
{
    public class GetGeoMapDetailsQuery : IRequest<GeoMapDetailsVm>
    {
        public Guid Id { get; set; }
    }
}

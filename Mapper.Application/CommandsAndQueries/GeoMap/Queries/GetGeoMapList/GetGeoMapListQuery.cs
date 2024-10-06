using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Queries.GetGeoMapList
{
    public class GetGeoMapListQuery : IRequest<GeoMapListVm>
    {
        public Guid UserId { get; set; }
    }
}

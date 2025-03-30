using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Application.CommandsAndQueries.GeoMark.Queries.GetGeoMarkList
{
    public class GeoMarkListVm
    {
        public required IList<GeoMarkLookupDto> GeoMarks { get; set; }
    }
}

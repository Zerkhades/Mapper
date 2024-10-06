using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Commands.DeleteGeoMapCommand
{
    public class ArchiveGeoMapCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}

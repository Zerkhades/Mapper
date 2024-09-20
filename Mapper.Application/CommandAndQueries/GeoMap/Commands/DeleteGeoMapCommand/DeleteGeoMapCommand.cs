using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Application.CommandAndQueries.GeoMap.Commands.DeleteGeoMapCommand
{
    public class DeleteGeoMapCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}

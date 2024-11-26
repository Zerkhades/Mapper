using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Mapper.Application.CommandsAndQueries.GeoMark.Commands.ArchiveGeoMarkCommand
{
    public class ArchiveGeoMarkCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}

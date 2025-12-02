using MediatR;

namespace Mapper.Application.CommandsAndQueries.GeoMark.Commands.ArchiveGeoMarkCommand
{
    public class ArchiveGeoMarkCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}

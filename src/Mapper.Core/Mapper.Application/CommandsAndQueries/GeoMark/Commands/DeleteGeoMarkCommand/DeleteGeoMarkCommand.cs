using MediatR;

namespace Mapper.Application.CommandsAndQueries.GeoMark.Commands.DeleteGeoMarkCommand
{
    public class DeleteGeoMarkCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}

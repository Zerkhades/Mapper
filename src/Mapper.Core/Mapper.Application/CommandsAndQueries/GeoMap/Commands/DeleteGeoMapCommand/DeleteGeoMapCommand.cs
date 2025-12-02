using MediatR;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Commands.DeleteGeoMapCommand
{
    public class DeleteGeoMapCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
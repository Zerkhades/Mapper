using MediatR;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Commands.CreateGeoMapCommand
{
    public class CreateGeoMapCommand : IRequest<Guid>
    {
        public Guid Id { get; set; }
        public string MapName { get; set; }
        public string MapDescription { get; set; }
        public IList<Domain.GeoMark>? GeoMarks { get; set; } = null;
        public bool IsArchived { get; set; }
    }
}

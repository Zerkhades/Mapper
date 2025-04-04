using MediatR;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Commands.UpdateGeoMapCommand
{
    public class UpdateGeoMapCommand : IRequest
    {
        public Guid Id { get; set; }
        public required string MapName { get; set; }
        public required string MapDescription { get; set; }
        //public byte[] Map { get; set; }
        public virtual IList<Domain.GeoMark>? GeoMarks { get; set; }
        public bool IsArchived { get; set; }
    }
}

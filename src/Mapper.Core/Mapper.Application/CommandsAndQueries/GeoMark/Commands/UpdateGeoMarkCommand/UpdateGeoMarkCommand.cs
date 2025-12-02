using MediatR;

namespace Mapper.Application.CommandsAndQueries.GeoMark.Commands.UpdateGeoMarkCommand
{
    public class UpdateGeoMarkCommand : IRequest
    {
        public Guid Id { get; set; }
        public Guid GeoMapId { get; set; }
        public virtual Domain.GeoMap? GeoMap { get; set; }
        public required string MarkName { get; set; }
        public string? MarkDescription { get; set; }
        public string Color { get; set; }
        public string Emoji { get; set; }
        public int? Size { get; set; }
        public bool IsEmoji { get; set; }
        public bool IsArchived { get; set; }
        public bool IsEditable { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? EditDate { get; set; }
        public Guid? EditedBy { get; set; }
        public virtual IList<Domain.Employee>? Employees { get; set; }
        public virtual IList<Domain.GeoPhoto>? GeoPhotos { get; set; }
    }
}

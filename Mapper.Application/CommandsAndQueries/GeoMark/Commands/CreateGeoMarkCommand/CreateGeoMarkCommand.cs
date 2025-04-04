using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Mapper.Application.CommandsAndQueries.GeoMark.Commands.CreateGeoMarkCommand
{
    public class CreateGeoMarkCommand : IRequest<Guid>
    {
        [Key]
        public Guid Id { get; set; }
        public required Guid GeoMapId { get; set; }
        public virtual Domain.GeoMap? GeoMap { get; set; }
        public required string MarkName { get; set; }
        public string? MarkDescription { get; set; }
        public double XPos { get; set; }
        public double YPos { get; set; }
        public string Color { get; set; } = "#FF0000";
        public string Emoji { get; set; } = "\uD83D\uDE00"; //😀
        public int? Size { get; set; } = 16;
        public bool IsEmoji { get; set; } = false;
        public bool IsArchived { get; set; }
        public bool IsEditable { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? EditDate { get; set; }
        public Guid? EditedBy { get; set; }
        public virtual IList<Domain.Employee>? Employees { get; set; }
        public virtual IList<Domain.GeoPhoto>? GeoPhotos { get; set; }
    }
}

using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Mapper.Domain;
using MediatR;

namespace Mapper.Application.CommandsAndQueries.GeoMark.Commands.CreateGeoMarkCommand
{
    public class CreateGeoMarkCommand : IRequest<Guid>
    {
        [Key]
        public Guid Id { get; set; }
        public Guid GeoMapId { get; set; }
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
        public virtual ObservableCollection<Employee>? Employees { get; set; }
        public virtual ObservableCollection<GeoPhoto>? GeoPhotos { get; set; }
    }
}

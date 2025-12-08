using System.ComponentModel.DataAnnotations;

namespace Mapper.WebApi.Models
{
    public class GeoMarkDto
    {
        public Guid Id { get; set; }
        public Guid GeoMapId { get; set; }
        public required string MarkName { get; set; }
        public string? MarkDescription { get; set; }
        public string Color { get; set; } = "#FF0000";
        public string Emoji { get; set; } = "\uD83D\uDE00";
        public int? Size { get; set; } = 16;
        public bool IsEmoji { get; set; }
        public bool IsArchived { get; set; }
        public bool IsEditable { get; set; }
    }
}

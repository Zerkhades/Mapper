using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;


namespace Mapper.Domain
{
    public class GeoMap
    {
        [Key]
        public Guid Id { get; set; }
        public required string MapName { get; set; }
        public required string MapDescription { get; set; }
        //public byte[] Map { get; set; }
        public bool IsArchived { get; set; }
        //public DateTime CreationDate { get; set; }
        public virtual IList<GeoMark>? GeoMarks { get; set; }
    }
}

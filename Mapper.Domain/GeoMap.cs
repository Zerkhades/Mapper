using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;


namespace Mapper.Domain
{
    public class GeoMap
    {
        [Key]
        public Guid Id { get; set; }
        public string MapName { get; set; }
        public string MapDescription { get; set; }
        //public byte[] Map { get; set; }
        public bool IsArchived { get; set; }
        //public virtual ObservableCollection<GeoMark>? GeoMarks { get; set; }

        //public GeoMap()
        //{
        //    IsArchived = false;
        //}
    }
}

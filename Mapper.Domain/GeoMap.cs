using System.Collections.ObjectModel;


namespace Mapper.Domain
{
    public class GeoMap
    {
        public int Id { get; set; }
        public string MapName { get; set; }
        public string MapDescription { get; set; }
        public byte[] Map { get; set; }
        public bool IsArchived { get; set; }
        public virtual ObservableCollection<GeoMark>? GeoMarks { get; set; }

        public GeoMap()
        {
            IsArchived = false;
        }
    }
}

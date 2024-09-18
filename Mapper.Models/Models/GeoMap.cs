using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Mapper.Models.Interfaces;

namespace Mapper.Models.Models
{
    public class GeoMap : IGeoMap
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

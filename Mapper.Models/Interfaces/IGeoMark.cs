using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Models.Interfaces
{
    public interface IGeoMark
    {
        public string MarkName { get; }
        public string? MarkDescription { get; }
        public string Color{ get; }
        public string? Emoji { get; }
        public int? Size { get; }
        public bool IsEmoji { get; }
        public bool IsArchived { get; }
        public int GeoMapId { get; }
        public ObservableCollection<Models.Employee>? Employees { get; }
        public ObservableCollection<Models.GeoPhoto>? GeoPhotos { get; }

    }
}

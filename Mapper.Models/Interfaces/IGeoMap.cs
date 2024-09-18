using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Models.Interfaces
{
    public interface IGeoMap
    {
        public int Id { get;}
        public string MapName { get;}
        public string MapDescription { get;}
        public byte[] Map { get;}
        public bool IsArchived { get;}
        public ObservableCollection<Models.GeoMark>? GeoMarks { get;}
    }
}

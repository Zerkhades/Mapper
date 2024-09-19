using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Domain
{
    public class GeoPhoto
    {
        public int Id { get; set; }
        public string PhotoName { get; set; }
        public byte[] File { get; set; }
        public bool IsArchived { get; set; }

        public GeoPhoto()
        {
            IsArchived = false;
        }

    }
}

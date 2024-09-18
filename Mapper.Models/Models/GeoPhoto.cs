using CommunityToolkit.Mvvm.ComponentModel;
using Mapper.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Models.Models
{
    public class GeoPhoto : IGeoPhoto
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

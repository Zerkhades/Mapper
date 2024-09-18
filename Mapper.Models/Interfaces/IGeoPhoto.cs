using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Models.Interfaces
{
    public interface IGeoPhoto
    {
        public int Id { get; }
        public string PhotoName { get; }
        public byte[] File { get; }
        public bool IsArchived { get; }
    }
}

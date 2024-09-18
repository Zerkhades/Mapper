using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Models.Interfaces
{
    internal interface IEmployeePhoto
    {
        public int Id { get;}
        public byte[] Photo { get;}
        public bool IsArchived { get;}
    }
}

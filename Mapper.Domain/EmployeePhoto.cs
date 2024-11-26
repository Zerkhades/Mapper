using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Domain
{
    public class EmployeePhoto
    {
        public Guid Id { get; set; }
        //public byte[] Photo { get; set; }
        public bool IsArchived { get; set; }

        public EmployeePhoto()
        {
            IsArchived = false;
        }
    }
}

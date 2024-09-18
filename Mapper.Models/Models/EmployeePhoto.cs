using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapper.Models.Interfaces;

namespace Mapper.Models.Models
{
    public class EmployeePhoto : IEmployeePhoto
    {
        public int Id { get; set; }
        public byte[] Photo { get; set; }
        public bool IsArchived { get; set; }

        public EmployeePhoto()
        {
            IsArchived = false;
        }
    }
}

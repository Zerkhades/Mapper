using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Application.CommandsAndQueries.Employee.Queries.GetEmployeeList
{
    public class EmployeeListVm
    {
        public required IList<EmployeeLookupDto> Employees { get; set; }
    }
}

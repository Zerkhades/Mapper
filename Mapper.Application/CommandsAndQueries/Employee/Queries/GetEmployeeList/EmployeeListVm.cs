namespace Mapper.Application.CommandsAndQueries.Employee.Queries.GetEmployeeList
{
    public class EmployeeListVm
    {
        public required IList<EmployeeLookupDto> Employees { get; set; }
    }
}

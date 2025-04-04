using MediatR;

namespace Mapper.Application.CommandsAndQueries.Employee.Queries.GetEmployeeList
{
    public class GetEmployeeListQuery : IRequest<EmployeeListVm>
    {
        public Guid UserId { get; set; }
    }
}

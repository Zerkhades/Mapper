using MediatR;

namespace Mapper.Application.CommandsAndQueries.Employee.Queries.GetEmployeeDetails
{
    public class GetEmployeeDetailsQuery : IRequest<EmployeeDetailsVm>
    {
        public Guid Id { get; set; }
    }
}

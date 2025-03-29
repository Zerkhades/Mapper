using MediatR;

namespace Mapper.Application.CommandsAndQueries.Employee.Commands.DeleteEmployeeCommand
{
    public class DeleteEmployeeCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}

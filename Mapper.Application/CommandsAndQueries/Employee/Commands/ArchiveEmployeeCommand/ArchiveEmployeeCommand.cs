using MediatR;

namespace Mapper.Application.CommandsAndQueries.Employee.Commands.ArchiveEmployeeCommand
{
    public class ArchiveEmployeeCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}

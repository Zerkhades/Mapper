using FluentValidation;

namespace Mapper.Application.CommandsAndQueries.Employee.Commands.ArchiveEmployeeCommand
{
    public class ArchiveEmployeeCommandValidator : AbstractValidator<ArchiveEmployeeCommand>
    {
        public ArchiveEmployeeCommandValidator()
        {
            RuleFor(deleteMapCommand => deleteMapCommand.Id).NotEqual(Guid.Empty);
        }
    }
}

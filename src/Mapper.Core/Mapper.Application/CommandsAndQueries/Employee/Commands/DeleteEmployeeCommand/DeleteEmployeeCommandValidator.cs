using FluentValidation;

namespace Mapper.Application.CommandsAndQueries.Employee.Commands.DeleteEmployeeCommand
{
    public class DeleteEmployeeCommandValidator : AbstractValidator<DeleteEmployeeCommand>
    {
        public DeleteEmployeeCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Employee Id is required.");
        }
    }
}


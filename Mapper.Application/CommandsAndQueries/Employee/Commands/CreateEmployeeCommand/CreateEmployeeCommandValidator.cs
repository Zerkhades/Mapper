using FluentValidation;


namespace Mapper.Application.CommandsAndQueries.Employee.Commands.CreateEmployeeCommand
{
    public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
    {
        public CreateEmployeeCommandValidator()
        {
            RuleFor(createEmployeeCommand => createEmployeeCommand.Id).NotEqual(Guid.Empty);
            RuleFor(createEmployeeCommand => createEmployeeCommand.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(createEmployeeCommand => createEmployeeCommand.Surname).NotEmpty().MaximumLength(100);
            //RuleFor(createEmployeeCommand => createEmployeeCommand.Email).NotEmpty().EmailAddress();
        }
    }
}

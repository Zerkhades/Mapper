using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Application.CommandsAndQueries.Employee.Commands.UpdateEmployeeCommand
{
    public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
    {
        public UpdateEmployeeCommandValidator()
        {
            RuleFor(updateEmployeeCommand => updateEmployeeCommand.Id).NotEqual(Guid.Empty);
            RuleFor(updateEmployeeCommand => updateEmployeeCommand.FirstName)
                .NotEmpty().MaximumLength(100);
            RuleFor(updateEmployeeCommand => updateEmployeeCommand.Surname)
                .NotEmpty().MaximumLength(100);
            RuleFor(updateEmployeeCommand => updateEmployeeCommand.Email)
                .NotEmpty().EmailAddress();
        }
    }
}

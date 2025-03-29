using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

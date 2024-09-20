using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Commands.UpdateGeoMapCommand
{
    internal class UpdateGeoMapCommandValidator : AbstractValidator<UpdateGeoMapCommand>
    {
        public UpdateGeoMapCommandValidator()
        {
            RuleFor(updateNoteCommand => updateNoteCommand.Id).NotEqual(Guid.Empty);
            RuleFor(updateNoteCommand => updateNoteCommand.MapName)
                .NotEmpty().MaximumLength(250);
        }
    }
}

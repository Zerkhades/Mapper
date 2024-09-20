using System;
using FluentValidation;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Commands.CreateGeoMapCommand
{
    internal class CreateGeoMapValidator : AbstractValidator<CreateGeoMapCommand>
    {
        public CreateGeoMapValidator()
        {
            RuleFor(createNoteCommand =>
                createNoteCommand.MapName).NotEmpty().MaximumLength(250);
            RuleFor(createNoteCommand =>
                createNoteCommand.Id).NotEqual(Guid.Empty);
        }
    }
}

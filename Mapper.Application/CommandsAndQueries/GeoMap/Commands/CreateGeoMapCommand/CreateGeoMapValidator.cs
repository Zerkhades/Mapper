using System;
using FluentValidation;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Commands.CreateGeoMapCommand
{
    internal class CreateGeoMapValidator : AbstractValidator<CreateGeoMapCommand>
    {
        public CreateGeoMapValidator()
        {
            RuleFor(createGeoMapCommand =>
                createGeoMapCommand.MapName).NotEmpty().MaximumLength(250);
            RuleFor(createGeoMapCommand =>
                createGeoMapCommand.Id).NotEqual(Guid.Empty);
        }
    }
}

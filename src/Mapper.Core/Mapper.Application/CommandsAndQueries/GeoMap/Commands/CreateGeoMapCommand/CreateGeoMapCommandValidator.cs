using System;
using FluentValidation;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Commands.CreateGeoMapCommand
{
    public class CreateGeoMapCommandValidator : AbstractValidator<CreateGeoMapCommand>
    {
        public CreateGeoMapCommandValidator()
        {
            RuleFor(createGeoMapCommand =>
                createGeoMapCommand.MapName).NotEmpty().MaximumLength(100);
            RuleFor(createGeoMapCommand =>
                createGeoMapCommand.Id).NotEqual(Guid.Empty);
        }
    }
}

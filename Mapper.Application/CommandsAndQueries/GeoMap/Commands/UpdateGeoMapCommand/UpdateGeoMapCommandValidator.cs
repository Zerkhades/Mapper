using FluentValidation;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Commands.UpdateGeoMapCommand
{
    public class UpdateGeoMapCommandValidator : AbstractValidator<UpdateGeoMapCommand>
    {
        public UpdateGeoMapCommandValidator()
        {
            RuleFor(updateGeoMapCommand => updateGeoMapCommand.Id).NotEqual(Guid.Empty);
            RuleFor(updateGeoMapCommand => updateGeoMapCommand.MapName)
                .NotEmpty().MaximumLength(250);
        }
    }
}

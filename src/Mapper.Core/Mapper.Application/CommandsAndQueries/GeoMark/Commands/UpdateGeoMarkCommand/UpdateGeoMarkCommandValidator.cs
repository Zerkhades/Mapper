using FluentValidation;

namespace Mapper.Application.CommandsAndQueries.GeoMark.Commands.UpdateGeoMarkCommand
{
    public class UpdateGeoMarkCommandValidator : AbstractValidator<UpdateGeoMarkCommand>
    {
        public UpdateGeoMarkCommandValidator()
        {
            RuleFor(updateGeoMarkCommand => updateGeoMarkCommand.Id).NotEqual(Guid.Empty);
            RuleFor(updateGeoMarkCommand => updateGeoMarkCommand.MarkName)
                .NotEmpty().MaximumLength(100);
            RuleFor(updateGeoMarkCommand => updateGeoMarkCommand.Color)
                .NotEmpty().MaximumLength(50);
            RuleFor(updateGeoMarkCommand => updateGeoMarkCommand.Emoji)
                .MaximumLength(10);
            RuleFor(updateGeoMarkCommand => updateGeoMarkCommand.Size)
                .GreaterThan(0).When(updateGeoMarkCommand => updateGeoMarkCommand.Size.HasValue);
        }
    }
}

using FluentValidation;

namespace Mapper.Application.CommandsAndQueries.GeoMark.Commands.DeleteGeoMarkCommand
{
    public class DeleteGeoMarkCommandValidator : AbstractValidator<DeleteGeoMarkCommand>
    {
        public DeleteGeoMarkCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("GeoMark Id is required.");
        }
    }
}

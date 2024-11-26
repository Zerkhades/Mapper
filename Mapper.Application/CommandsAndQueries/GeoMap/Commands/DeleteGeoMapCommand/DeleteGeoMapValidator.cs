using FluentValidation;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Commands.DeleteGeoMapCommand
{
    public class DeleteGeoMapCommandValidator : AbstractValidator<DeleteGeoMapCommand>
    {
        public DeleteGeoMapCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("GeoMap Id is required.");
        }
    }
}
using FluentValidation;

namespace Mapper.Application.CommandsAndQueries.GeoMap.Commands.DeleteGeoMapCommand
{
    public class ArchiveGeoMapCommandValidator : AbstractValidator<ArchiveGeoMapCommand>
    {
        public ArchiveGeoMapCommandValidator()
        {
            RuleFor(deleteMapCommand => deleteMapCommand.Id).NotEqual(Guid.Empty);
        }
    }
}

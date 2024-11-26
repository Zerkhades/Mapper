using FluentValidation;


namespace Mapper.Application.CommandsAndQueries.GeoMark.Commands.ArchiveGeoMarkCommand
{
    public class ArchiveGeoMarkCommandValidator : AbstractValidator<ArchiveGeoMarkCommand>
    {
        public ArchiveGeoMarkCommandValidator() =>
            RuleFor(archiveMarkCommand => archiveMarkCommand.Id).NotEqual(Guid.Empty);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Mapper.Application.CommandsAndQueries.GeoMark.Commands.CreateGeoMarkCommand
{
    public class CreateGeoMarkCommandValidator : AbstractValidator<CreateGeoMarkCommand>
    {
        public CreateGeoMarkCommandValidator()
        {
            RuleFor(createGeoMarkCommand =>
                createGeoMarkCommand.XPos).NotNull();
            RuleFor(createGeoMarkCommand =>
                createGeoMarkCommand.YPos).NotNull();
            RuleFor(createGeoMarkCommand =>
                createGeoMarkCommand.MarkName).NotEmpty();
            RuleFor(createGeoMarkCommand =>
                createGeoMarkCommand.MarkDescription).NotEmpty();
            RuleFor(createGeoMarkCommand =>
                createGeoMarkCommand.Color).NotEmpty();
            RuleFor(createGeoMarkCommand =>
                createGeoMarkCommand.Emoji).NotEmpty();
            RuleFor(createGeoMarkCommand =>
                createGeoMarkCommand.GeoMapId).NotEqual(Guid.Empty);
            RuleFor(createGeoMarkCommand =>
                createGeoMarkCommand.Id).NotEqual(Guid.Empty);
        }
    }
}

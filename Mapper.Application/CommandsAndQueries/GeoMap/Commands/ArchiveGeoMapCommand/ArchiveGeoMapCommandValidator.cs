using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

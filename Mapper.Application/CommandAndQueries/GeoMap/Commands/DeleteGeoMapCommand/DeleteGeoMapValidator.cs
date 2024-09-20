using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Mapper.Application.CommandAndQueries.GeoMap.Commands.DeleteGeoMapCommand
{
    internal class DeleteGeoMapValidator : AbstractValidator<DeleteGeoMapCommand>
    {
        public DeleteGeoMapValidator()
        {
            RuleFor(deleteMapCommand => deleteMapCommand.Id).NotEqual(Guid.Empty);
        }
    }
}

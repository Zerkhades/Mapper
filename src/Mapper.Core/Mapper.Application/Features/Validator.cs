using FluentValidation;
using Mapper.Application.Features.GeoMaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Application.Features
{
    public class CreateGeoMapValidator : AbstractValidator<CreateGeoMapCommand>
    {
        public CreateGeoMapValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.ImageWidth).GreaterThan(0);
            RuleFor(x => x.ImageHeight).GreaterThan(0);
            RuleFor(x => x.ContentType).Must(ct => ct is "image/png" or "image/jpeg");
        }
    }

}

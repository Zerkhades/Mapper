using FluentValidation;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;

namespace Mapper.Application.Features.GeoMaps.Commands.CreateGeoMap
{
    public record CreateGeoMapCommand(
        string Name,
        string? Description,
        Stream ImageStream,
        string FileName,
        string ContentType,
        int ImageWidth,
        int ImageHeight
    ) : IRequest<Guid>;

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

    public class CreateGeoMapHandler : IRequestHandler<CreateGeoMapCommand, Guid>
    {
        private readonly IMapperDbContext _db;
        private readonly IMapImageStorage _storage;

        public CreateGeoMapHandler(IMapperDbContext db, IMapImageStorage storage)
        {
            _db = db;
            _storage = storage;
        }

        public async Task<Guid> Handle(CreateGeoMapCommand request, CancellationToken ct)
        {
            var path = await _storage.SaveAsync(request.ImageStream, request.FileName, request.ContentType, ct);
            var map = new GeoMap(request.Name, path, request.ImageWidth, request.ImageHeight, request.Description);

            _db.GeoMaps.Add(map);
            await _db.SaveChangesAsync(ct);

            return map.Id;
        }
    }
}

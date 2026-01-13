using FluentValidation;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.GeoMarks.Commands.AddCameraMark
{
    public record AddCameraMarkCommand(
        Guid GeoMapId,
        double X,
        double Y,
        string Title,
        string? Description,
        string? CameraName,
        string? StreamUrl
    ) : IRequest<Guid>;

    public class AddCameraMarkValidator : AbstractValidator<AddCameraMarkCommand>
    {
        public AddCameraMarkValidator()
        {
            RuleFor(x => x.GeoMapId).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.X).InclusiveBetween(0, 1);
            RuleFor(x => x.Y).InclusiveBetween(0, 1);
            RuleFor(x => x.StreamUrl)
                .Must(url => string.IsNullOrWhiteSpace(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("StreamUrl must be a valid absolute URI");
        }
    }

    public class AddCameraMarkHandler : IRequestHandler<AddCameraMarkCommand, Guid>
    {
        private readonly IMapperDbContext _db;
        private readonly ICacheService _cache;
        private readonly IMapRealtimeNotifier _notifier;

        public AddCameraMarkHandler(IMapperDbContext db, ICacheService cache, IMapRealtimeNotifier notifier)
        {
            _db = db; _cache = cache; _notifier = notifier;
        }

        public async Task<Guid> Handle(AddCameraMarkCommand r, CancellationToken ct)
        {
            var exists = await _db.GeoMaps.AnyAsync(x => x.Id == r.GeoMapId, ct);
            if (!exists) throw new NotFoundException($"GeoMap {r.GeoMapId} not found", r.GeoMapId);

            var mark = new CameraMark(r.GeoMapId, r.X, r.Y, r.Title, r.CameraName, r.StreamUrl, r.Description);

            _db.GeoMarks.Add(mark);
            await _db.SaveChangesAsync(ct);

            await _cache.RemoveAsync($"geomap:{r.GeoMapId}", ct);

            await _notifier.MarkAdded(r.GeoMapId, new
            {
                id = mark.Id,
                type = mark.Type,
                x = mark.X,
                y = mark.Y,
                title = mark.Title,
                description = mark.Description,
                cameraName = mark.CameraName,
                streamUrl = mark.StreamUrl
            }, ct);

            return mark.Id;
        }
    }
}

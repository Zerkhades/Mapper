using FluentValidation;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Application.Features.GeoMarks.Commands.CameraMarkCommands
{
    public record UpdateCameraMarkCommand(
        Guid GeoMapId,
        Guid MarkId,
        float X,
        float Y,
        string Title,
        string? Description,
        string? CameraName,
        string? StreamUrl
    ) : IRequest;

    public class UpdateCameraMarkValidator : AbstractValidator<UpdateCameraMarkCommand>
    {
        public UpdateCameraMarkValidator()
        {
            RuleFor(x => x.GeoMapId).NotEmpty();
            RuleFor(x => x.MarkId).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.X).InclusiveBetween(0f, 1f);
            RuleFor(x => x.Y).InclusiveBetween(0f, 1f);

            RuleFor(x => x.StreamUrl)
                .Must(url => string.IsNullOrWhiteSpace(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("StreamUrl must be a valid absolute URI");
        }
    }

    public class UpdateCameraMarkHandler : IRequestHandler<UpdateCameraMarkCommand>
    {
        private readonly IMapperDbContext _db;
        private readonly ICacheService _cache;
        private readonly IMapRealtimeNotifier _notifier;

        public UpdateCameraMarkHandler(IMapperDbContext db, ICacheService cache, IMapRealtimeNotifier notifier)
        {
            _db = db; _cache = cache; _notifier = notifier;
        }

        public async Task Handle(UpdateCameraMarkCommand r, CancellationToken ct)
        {
            var mark = await _db.GeoMarks
                .OfType<CameraMark>()
                .FirstOrDefaultAsync(x => x.Id == r.MarkId && x.GeoMapId == r.GeoMapId, ct);

            if (mark is null)
                throw new NotFoundException($"CameraMark {r.MarkId} not found on map {r.GeoMapId}", r.MarkId);

            mark.Move(r.X, r.Y);
            mark.UpdateText(r.Title, r.Description);
            mark.UpdateCamera(r.CameraName, r.StreamUrl);

            await _db.SaveChangesAsync(ct);

            await _cache.RemoveAsync($"geomap:{r.GeoMapId}", ct);

            await _notifier.MarkUpdated(r.GeoMapId, new
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
        }
    }
}

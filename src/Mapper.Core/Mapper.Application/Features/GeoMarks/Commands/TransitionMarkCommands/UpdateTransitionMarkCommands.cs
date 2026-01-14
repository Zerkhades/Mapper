using FluentValidation;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.GeoMarks.Commands.TransitionMarkCommands
{
    public record UpdateTransitionMarkCommand(
        Guid GeoMapId,
        Guid MarkId,
        double X,
        double Y,
        string Title,
        string? Description,
        Guid TargetGeoMapId
    ) : IRequest;

    public class UpdateTransitionMarkValidator : AbstractValidator<UpdateTransitionMarkCommand>
    {
        public UpdateTransitionMarkValidator()
        {
            RuleFor(x => x.GeoMapId).NotEmpty();
            RuleFor(x => x.MarkId).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.X).InclusiveBetween(0f, 1f);
            RuleFor(x => x.Y).InclusiveBetween(0f, 1f);
            RuleFor(x => x.TargetGeoMapId).NotEmpty();
        }
    }

    public class UpdateTransitionMarkHandler : IRequestHandler<UpdateTransitionMarkCommand>
    {
        private readonly IMapperDbContext _db;
        private readonly ICacheService _cache;
        private readonly IMapRealtimeNotifier _notifier;

        public UpdateTransitionMarkHandler(IMapperDbContext db, ICacheService cache, IMapRealtimeNotifier notifier)
        {
            _db = db; _cache = cache; _notifier = notifier;
        }

        public async Task Handle(UpdateTransitionMarkCommand r, CancellationToken ct)
        {
            var mark = await _db.GeoMarks
                .OfType<TransitionMark>()
                .FirstOrDefaultAsync(x => x.Id == r.MarkId && x.GeoMapId == r.GeoMapId, ct);

            if (mark is null)
                throw new NotFoundException($"TransitionMark {r.MarkId} not found on map {r.GeoMapId}", r.MarkId);

            mark.Move(r.X, r.Y);
            mark.UpdateText(r.Title, r.Description);
            mark.SetTarget(r.TargetGeoMapId);

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
                targetGeoMapId = mark.TargetGeoMapId
            }, ct);
        }
    }
}

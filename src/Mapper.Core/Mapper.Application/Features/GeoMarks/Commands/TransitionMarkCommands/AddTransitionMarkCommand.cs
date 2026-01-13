using FluentValidation;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Mapper.Application.Common.Exceptions;

namespace Mapper.Application.Features.GeoMarks.Commands.TransitionMarkCommands
{
    public record AddTransitionMarkCommand(
        Guid GeoMapId,
        double X,
        double Y,
        string Title,
        string? Description,
        Guid TargetGeoMapId
    ) : IRequest<Guid>;

    public class AddTransitionMarkValidator : AbstractValidator<AddTransitionMarkCommand>
    {
        public AddTransitionMarkValidator()
        {
            RuleFor(x => x.GeoMapId).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.X).InclusiveBetween(0, 1);
            RuleFor(x => x.Y).InclusiveBetween(0, 1);
            RuleFor(x => x.TargetGeoMapId).NotEmpty();
        }
    }

    public class AddTransitionMarkHandler : IRequestHandler<AddTransitionMarkCommand, Guid>
    {
        private readonly IMapperDbContext _db;
        private readonly ICacheService _cache;
        private readonly IMapRealtimeNotifier _notifier;

        public AddTransitionMarkHandler(IMapperDbContext db, ICacheService cache, IMapRealtimeNotifier notifier)
        {
            _db = db; _cache = cache; _notifier = notifier;
        }

        public async Task<Guid> Handle(AddTransitionMarkCommand r, CancellationToken ct)
        {
            var exists = await _db.GeoMaps.AnyAsync(x => x.Id == r.GeoMapId, ct);
            if (!exists) throw new NotFoundException($"GeoMap {r.GeoMapId} not found", r.GeoMapId);

            var mark = new TransitionMark(r.GeoMapId, r.X, r.Y, r.Title, r.TargetGeoMapId, r.Description);

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
                targetGeoMapId = mark.TargetGeoMapId
            }, ct);

            return mark.Id;
        }
    }
}

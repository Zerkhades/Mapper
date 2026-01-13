using FluentValidation;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.GeoMarks.Commands.GeoMarkCommands
{
    public record AddGeoMarkCommand(
        Guid GeoMapId,
        GeoMarkType Type,
        double X,
        double Y,
        string Title,
        string? Description,
        Guid? TargetGeoMapId,
        string? WorkplaceCode,
        IReadOnlyList<Guid>? EmployeeIds,
        string? CameraName,
        string? StreamUrl
    ) : IRequest<Guid>;

    public class AddGeoMarkHandler : IRequestHandler<AddGeoMarkCommand, Guid>
    {
        private readonly IMapperDbContext _db;
        private readonly IMapRealtimeNotifier _notifier;

        public AddGeoMarkHandler(IMapperDbContext db, IMapRealtimeNotifier notifier)
        {
            _db = db;
            _notifier = notifier;
        }

        public async Task<Guid> Handle(AddGeoMarkCommand r, CancellationToken ct)
        {
            var mapExists = await _db.GeoMaps.AnyAsync(x => x.Id == r.GeoMapId, ct);
            if (!mapExists) throw new NotFoundException($"GeoMap {r.GeoMapId} not found", r.GeoMapId);

            GeoMark mark = r.Type switch
            {
                GeoMarkType.Transition => new TransitionMark(r.GeoMapId, r.X, r.Y, r.Title,
                    r.TargetGeoMapId ?? throw new ValidationException("TargetGeoMapId is required")),

                GeoMarkType.Workplace => CreateWorkplace(r),

                GeoMarkType.Camera => new CameraMark(r.GeoMapId, r.X, r.Y, r.Title, r.CameraName, r.StreamUrl, r.Description),

                _ => throw new ValidationException($"Unsupported mark type: {r.Type}")
            };

            _db.GeoMarks.Add(mark);
            await _db.SaveChangesAsync(ct);
            await _notifier.MarkAdded(r.GeoMapId, new
            {
                id = mark.Id,
                type = mark.Type,
                x = mark.X,
                y = mark.Y,
                title = mark.Title
            }, ct);

            return mark.Id;
        }

        private static WorkplaceMark CreateWorkplace(AddGeoMarkCommand r)
        {
            var wm = new WorkplaceMark(
                r.GeoMapId, r.X, r.Y, r.Title,
                r.WorkplaceCode ?? throw new ValidationException("WorkplaceCode is required"),
                r.Description);

            if (r.EmployeeIds is { Count: > 0 })
                foreach (var id in r.EmployeeIds.Distinct())
                    wm.Employees.Add(new WorkplaceEmployee(wm.Id, id));

            return wm;
        }
    }
}

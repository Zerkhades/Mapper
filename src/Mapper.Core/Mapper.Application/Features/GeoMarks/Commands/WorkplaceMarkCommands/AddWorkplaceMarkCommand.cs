using FluentValidation;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.GeoMarks.Commands.WorkplaceMarkCommands
{
    public record AddWorkplaceMarkCommand(
        Guid GeoMapId,
        double X,
        double Y,
        string Title,
        string? Description,
        string WorkplaceCode,
        IReadOnlyList<Guid>? EmployeeIds
    ) : IRequest<Guid>;

    public class AddWorkplaceMarkValidator : AbstractValidator<AddWorkplaceMarkCommand>
    {
        public AddWorkplaceMarkValidator()
        {
            RuleFor(x => x.GeoMapId).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.X).InclusiveBetween(0, 1);
            RuleFor(x => x.Y).InclusiveBetween(0, 1);
            RuleFor(x => x.WorkplaceCode).NotEmpty().MaximumLength(64);
        }
    }

    public class AddWorkplaceMarkHandler : IRequestHandler<AddWorkplaceMarkCommand, Guid>
    {
        private readonly IMapperDbContext _db;
        private readonly ICacheService _cache;
        private readonly IMapRealtimeNotifier _notifier;

        public AddWorkplaceMarkHandler(IMapperDbContext db, ICacheService cache, IMapRealtimeNotifier notifier)
        {
            _db = db; _cache = cache; _notifier = notifier;
        }

        public async Task<Guid> Handle(AddWorkplaceMarkCommand r, CancellationToken ct)
        {
            var exists = await _db.GeoMaps.AnyAsync(x => x.Id == r.GeoMapId, ct);
            if (!exists) throw new NotFoundException($"GeoMap {r.GeoMapId} not found", r.GeoMapId);

            var mark = new WorkplaceMark(r.GeoMapId, r.X, r.Y, r.Title, r.WorkplaceCode, r.Description);

            if (r.EmployeeIds is { Count: > 0 })
            {
                foreach (var empId in r.EmployeeIds.Distinct())
                    mark.Employees.Add(new WorkplaceEmployee(mark.Id, empId));
            }

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
                workplaceCode = mark.WorkplaceCode,
                employeeIds = mark.Employees.Select(e => e.EmployeeId).ToList()
            }, ct);

            return mark.Id;
        }
    }
}

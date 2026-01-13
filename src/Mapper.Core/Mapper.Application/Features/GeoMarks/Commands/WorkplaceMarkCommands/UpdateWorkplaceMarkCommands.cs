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

namespace Mapper.Application.Features.GeoMarks.Commands.WorkplaceMarkCommands
{
    public record UpdateWorkplaceMarkCommand(
        Guid GeoMapId,
        Guid MarkId,
        float X,
        float Y,
        string Title,
        string? Description,
        string WorkplaceCode,
        IReadOnlyList<Guid>? EmployeeIds
    ) : IRequest;

    public class UpdateWorkplaceMarkValidator : AbstractValidator<UpdateWorkplaceMarkCommand>
    {
        public UpdateWorkplaceMarkValidator()
        {
            RuleFor(x => x.GeoMapId).NotEmpty();
            RuleFor(x => x.MarkId).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.X).InclusiveBetween(0f, 1f);
            RuleFor(x => x.Y).InclusiveBetween(0f, 1f);
            RuleFor(x => x.WorkplaceCode).NotEmpty().MaximumLength(64);
        }
    }

    public class UpdateWorkplaceMarkHandler : IRequestHandler<UpdateWorkplaceMarkCommand>
    {
        private readonly IMapperDbContext _db;
        private readonly ICacheService _cache;
        private readonly IMapRealtimeNotifier _notifier;

        public UpdateWorkplaceMarkHandler(IMapperDbContext db, ICacheService cache, IMapRealtimeNotifier notifier)
        {
            _db = db; _cache = cache; _notifier = notifier;
        }

        public async Task Handle(UpdateWorkplaceMarkCommand r, CancellationToken ct)
        {
            var mark = await _db.GeoMarks
                .OfType<WorkplaceMark>()
                .Include(x => x.Employees)
                .FirstOrDefaultAsync(x => x.Id == r.MarkId && x.GeoMapId == r.GeoMapId, ct);

            if (mark is null)
                throw new NotFoundException($"WorkplaceMark {r.MarkId} not found on map {r.GeoMapId}", r.MarkId);

            mark.Move(r.X, r.Y);
            mark.UpdateText(r.Title, r.Description);
            mark.SetWorkplaceCode(r.WorkplaceCode);

            // обновление списка сотрудников: replace all
            var newIds = (r.EmployeeIds ?? Array.Empty<Guid>()).Distinct().ToHashSet();

            // remove
            var toRemove = mark.Employees.Where(e => !newIds.Contains(e.EmployeeId)).ToList();
            foreach (var e in toRemove) mark.Employees.Remove(e);

            // add
            var existing = mark.Employees.Select(e => e.EmployeeId).ToHashSet();
            foreach (var id in newIds)
                if (!existing.Contains(id))
                    mark.Employees.Add(new WorkplaceEmployee(mark.Id, id));

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
                workplaceCode = mark.WorkplaceCode,
                employeeIds = mark.Employees.Select(e => e.EmployeeId).ToList()
            }, ct);
        }
    }
}

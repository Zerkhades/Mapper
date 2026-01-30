using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.CameraArchive.Commands;

public class CreateCameraStatusHistoryHandler 
    : IRequestHandler<CreateCameraStatusHistoryCommand, Guid>
{
    private readonly IMapperDbContext _db;

    public CreateCameraStatusHistoryHandler(IMapperDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> Handle(CreateCameraStatusHistoryCommand request, CancellationToken ct)
    {
        var cameraExists = await _db.GeoMarks
            .OfType<CameraMark>()
            .AnyAsync(x => x.Id == request.CameraMarkId, ct);

        if (!cameraExists)
            throw new NotFoundException($"Camera {request.CameraMarkId} not found", request.CameraMarkId);

        // Get last status to calculate duration
        var lastStatus = await _db.CameraStatusHistories
            .Where(x => x.CameraMarkId == request.CameraMarkId)
            .OrderByDescending(x => x.ChangedAt)
            .FirstOrDefaultAsync(ct);

        var statusHistory = new CameraStatusHistory(
            request.CameraMarkId,
            request.IsOnline,
            request.Reason,
            request.Details,
            request.ResponseTimeMs
        );

        if (lastStatus != null)
        {
            var duration = statusHistory.ChangedAt - lastStatus.ChangedAt;
            lastStatus.SetDuration(duration);
        }

        _db.CameraStatusHistories.Add(statusHistory);
        await _db.SaveChangesAsync(ct);

        return statusHistory.Id;
    }
}

public class SetCameraStatusHistoryDurationHandler 
    : IRequestHandler<SetCameraStatusHistoryDurationCommand>
{
    private readonly IMapperDbContext _db;

    public SetCameraStatusHistoryDurationHandler(IMapperDbContext db)
    {
        _db = db;
    }

    public async Task Handle(SetCameraStatusHistoryDurationCommand request, CancellationToken ct)
    {
        var statusHistory = await _db.CameraStatusHistories
            .FirstOrDefaultAsync(x => x.Id == request.StatusHistoryId, ct);

        if (statusHistory is null)
            throw new NotFoundException($"Status history {request.StatusHistoryId} not found", request.StatusHistoryId);

        statusHistory.SetDuration(request.Duration);
        await _db.SaveChangesAsync(ct);
    }
}

public class DeleteOldCameraStatusHistoryHandler 
    : IRequestHandler<DeleteOldCameraStatusHistoryCommand>
{
    private readonly IMapperDbContext _db;

    public DeleteOldCameraStatusHistoryHandler(IMapperDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DeleteOldCameraStatusHistoryCommand request, CancellationToken ct)
    {
        var cutoffDate = DateTimeOffset.UtcNow.AddDays(-request.DaysToKeep);

        var oldRecords = await _db.CameraStatusHistories
            .Where(x => x.CameraMarkId == request.CameraMarkId && x.ChangedAt < cutoffDate)
            .ToListAsync(ct);

        _db.CameraStatusHistories.RemoveRange(oldRecords);
        await _db.SaveChangesAsync(ct);
    }
}

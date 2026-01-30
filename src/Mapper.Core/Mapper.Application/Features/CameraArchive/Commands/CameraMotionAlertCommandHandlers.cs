using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.CameraArchive.Commands;

public class CreateCameraMotionAlertHandler 
    : IRequestHandler<CreateCameraMotionAlertCommand, Guid>
{
    private readonly IMapperDbContext _db;

    public CreateCameraMotionAlertHandler(IMapperDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> Handle(CreateCameraMotionAlertCommand request, CancellationToken ct)
    {
        var cameraExists = await _db.GeoMarks
            .OfType<CameraMark>()
            .AnyAsync(x => x.Id == request.CameraMarkId, ct);

        if (!cameraExists)
            throw new NotFoundException($"Camera {request.CameraMarkId} not found", request.CameraMarkId);

        var alert = new CameraMotionAlert(
            request.CameraMarkId,
            request.Severity,
            request.MotionPercentage,
            request.SnapshotPath
        );

        _db.CameraMotionAlerts.Add(alert);
        await _db.SaveChangesAsync(ct);

        return alert.Id;
    }
}

public class ConfirmCameraMotionAlertHandler 
    : IRequestHandler<ConfirmCameraMotionAlertCommand>
{
    private readonly IMapperDbContext _db;

    public ConfirmCameraMotionAlertHandler(IMapperDbContext db)
    {
        _db = db;
    }

    public async Task Handle(ConfirmCameraMotionAlertCommand request, CancellationToken ct)
    {
        var alert = await _db.CameraMotionAlerts
            .FirstOrDefaultAsync(x => x.Id == request.AlertId, ct);

        if (alert is null)
            throw new NotFoundException($"Motion alert {request.AlertId} not found", request.AlertId);

        alert.Confirm();
        await _db.SaveChangesAsync(ct);
    }
}

public class ResolveCameraMotionAlertHandler 
    : IRequestHandler<ResolveCameraMotionAlertCommand>
{
    private readonly IMapperDbContext _db;

    public ResolveCameraMotionAlertHandler(IMapperDbContext db)
    {
        _db = db;
    }

    public async Task Handle(ResolveCameraMotionAlertCommand request, CancellationToken ct)
    {
        var alert = await _db.CameraMotionAlerts
            .FirstOrDefaultAsync(x => x.Id == request.AlertId, ct);

        if (alert is null)
            throw new NotFoundException($"Motion alert {request.AlertId} not found", request.AlertId);

        alert.Resolve(request.ResolutionNotes);
        await _db.SaveChangesAsync(ct);
    }
}

public class LinkMotionAlertToVideoHandler 
    : IRequestHandler<LinkMotionAlertToVideoCommand>
{
    private readonly IMapperDbContext _db;

    public LinkMotionAlertToVideoHandler(IMapperDbContext db)
    {
        _db = db;
    }

    public async Task Handle(LinkMotionAlertToVideoCommand request, CancellationToken ct)
    {
        var alert = await _db.CameraMotionAlerts
            .FirstOrDefaultAsync(x => x.Id == request.AlertId, ct);

        if (alert is null)
            throw new NotFoundException($"Motion alert {request.AlertId} not found", request.AlertId);

        var video = await _db.CameraVideoArchives
            .FirstOrDefaultAsync(x => x.Id == request.VideoArchiveId, ct);

        if (video is null)
            throw new NotFoundException($"Video archive {request.VideoArchiveId} not found", request.VideoArchiveId);

        alert.LinkToVideo(video.Id);
        await _db.SaveChangesAsync(ct);
    }
}

public class DeleteCameraMotionAlertHandler 
    : IRequestHandler<DeleteCameraMotionAlertCommand>
{
    private readonly IMapperDbContext _db;

    public DeleteCameraMotionAlertHandler(IMapperDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DeleteCameraMotionAlertCommand request, CancellationToken ct)
    {
        var alert = await _db.CameraMotionAlerts
            .FirstOrDefaultAsync(x => x.Id == request.AlertId, ct);

        if (alert is null)
            throw new NotFoundException($"Motion alert {request.AlertId} not found", request.AlertId);

        _db.CameraMotionAlerts.Remove(alert);
        await _db.SaveChangesAsync(ct);
    }
}

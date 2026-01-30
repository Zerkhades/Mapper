using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Application.Features.CameraArchive.Commands;

public class CreateCameraVideoArchiveHandler 
    : IRequestHandler<CreateCameraVideoArchiveCommand, Guid>
{
    private readonly IMapperDbContext _db;

    public CreateCameraVideoArchiveHandler(IMapperDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> Handle(CreateCameraVideoArchiveCommand request, CancellationToken ct)
    {
        var cameraExists = await _db.GeoMarks
            .OfType<CameraMark>()
            .AnyAsync(x => x.Id == request.CameraMarkId, ct);

        if (!cameraExists)
            throw new NotFoundException($"Camera {request.CameraMarkId} not found", request.CameraMarkId);

        var archive = new CameraVideoArchive(
            request.CameraMarkId,
            request.VideoPath,
            request.Duration,
            request.FileSizeBytes,
            request.Resolution,
            request.FramesPerSecond,
            request.ThumbnailPath
        );

        _db.CameraVideoArchives.Add(archive);
        await _db.SaveChangesAsync(ct);

        return archive.Id;
    }
}

public class MarkVideoArchiveAsArchivedHandler 
    : IRequestHandler<MarkVideoArchiveAsArchivedCommand>
{
    private readonly IMapperDbContext _db;

    public MarkVideoArchiveAsArchivedHandler(IMapperDbContext db)
    {
        _db = db;
    }

    public async Task Handle(MarkVideoArchiveAsArchivedCommand request, CancellationToken ct)
    {
        var archive = await _db.CameraVideoArchives
            .FirstOrDefaultAsync(x => x.Id == request.VideoArchiveId, ct);

        if (archive is null)
            throw new NotFoundException($"Video archive {request.VideoArchiveId} not found", request.VideoArchiveId);

        archive.Archive();
        await _db.SaveChangesAsync(ct);
    }
}

public class DeleteCameraVideoArchiveHandler 
    : IRequestHandler<DeleteCameraVideoArchiveCommand>
{
    private readonly IMapperDbContext _db;

    public DeleteCameraVideoArchiveHandler(IMapperDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DeleteCameraVideoArchiveCommand request, CancellationToken ct)
    {
        var archive = await _db.CameraVideoArchives
            .FirstOrDefaultAsync(x => x.Id == request.VideoArchiveId, ct);

        if (archive is null)
            throw new NotFoundException($"Video archive {request.VideoArchiveId} not found", request.VideoArchiveId);

        _db.CameraVideoArchives.Remove(archive);
        await _db.SaveChangesAsync(ct);
    }
}

public class UpdateVideoArchiveMotionDetectionHandler 
    : IRequestHandler<UpdateVideoArchiveMotionDetectionCommand>
{
    private readonly IMapperDbContext _db;

    public UpdateVideoArchiveMotionDetectionHandler(IMapperDbContext db)
    {
        _db = db;
    }

    public async Task Handle(UpdateVideoArchiveMotionDetectionCommand request, CancellationToken ct)
    {
        var archive = await _db.CameraVideoArchives
            .FirstOrDefaultAsync(x => x.Id == request.VideoArchiveId, ct);

        if (archive is null)
            throw new NotFoundException($"Video archive {request.VideoArchiveId} not found", request.VideoArchiveId);

        archive.SetMotionDetected(request.HasMotion);
        await _db.SaveChangesAsync(ct);
    }
}

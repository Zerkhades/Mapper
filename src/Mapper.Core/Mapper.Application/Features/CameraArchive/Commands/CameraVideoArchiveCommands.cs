using Mapper.Domain;
using MediatR;

namespace Mapper.Application.Features.CameraArchive.Commands;

public record CreateCameraVideoArchiveCommand(
    Guid CameraMarkId,
    string VideoPath,
    TimeSpan Duration,
    long FileSizeBytes,
    string Resolution,
    int FramesPerSecond,
    string? ThumbnailPath = null
) : IRequest<Guid>;

public record MarkVideoArchiveAsArchivedCommand(Guid VideoArchiveId) : IRequest;

public record DeleteCameraVideoArchiveCommand(Guid VideoArchiveId) : IRequest;

public record UpdateVideoArchiveMotionDetectionCommand(Guid VideoArchiveId, bool HasMotion) : IRequest;

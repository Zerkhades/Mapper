using Mapper.Domain;
using MediatR;

namespace Mapper.Application.Features.CameraArchive.Commands;

public record CreateCameraMotionAlertCommand(
    Guid CameraMarkId,
    MotionSeverity Severity,
    double MotionPercentage,
    string? SnapshotPath = null
) : IRequest<Guid>;

public record ConfirmCameraMotionAlertCommand(Guid AlertId) : IRequest;

public record ResolveCameraMotionAlertCommand(Guid AlertId, string? ResolutionNotes = null) : IRequest;

public record LinkMotionAlertToVideoCommand(Guid AlertId, Guid VideoArchiveId) : IRequest;

public record DeleteCameraMotionAlertCommand(Guid AlertId) : IRequest;

using Mapper.Domain;
using MediatR;

namespace Mapper.Application.Features.CameraArchive.Commands;

public record CreateCameraStatusHistoryCommand(
    Guid CameraMarkId,
    bool IsOnline,
    CameraStatusReason Reason,
    string? Details = null,
    int? ResponseTimeMs = null
) : IRequest<Guid>;

public record SetCameraStatusHistoryDurationCommand(Guid StatusHistoryId, TimeSpan Duration) : IRequest;

public record DeleteOldCameraStatusHistoryCommand(Guid CameraMarkId, int DaysToKeep = 90) : IRequest;

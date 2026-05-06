using Mapper.Application.Features.Retention.DTOs;
using MediatR;

namespace Mapper.Application.Features.Retention.Commands;

public record CleanupArchiveRetentionCommand(
    Guid? CameraMarkId = null,
    DateTimeOffset? Now = null,
    int MotionVideoRetentionDays = 90,
    int NoMotionVideoRetentionDays = 7,
    int ArchivedVideoRetentionDays = 365,
    int Take = 100,
    bool DryRun = true,
    bool Confirm = false)
    : IRequest<ArchiveRetentionCleanupResultDto>;

using Mapper.Application.Features.Retention.DTOs;
using MediatR;

namespace Mapper.Application.Features.Retention.Queries;

public record GetArchiveRetentionPreviewQuery(
    Guid? CameraMarkId = null,
    DateTimeOffset? Now = null,
    int MotionVideoRetentionDays = 90,
    int NoMotionVideoRetentionDays = 7,
    int ArchivedVideoRetentionDays = 365,
    int Take = 100)
    : IRequest<ArchiveRetentionPreviewDto>;

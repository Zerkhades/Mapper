using Mapper.Application.Features.DTOs;
using MediatR;

namespace Mapper.Application.Features.CameraArchive.Queries;

public record GetCameraMotionAlertsQuery(Guid CameraMarkId, int Skip = 0, int Take = 50)
    : IRequest<List<CameraMotionAlertListItemDto>>;

public record GetCameraMotionAlertByIdQuery(Guid CameraMarkId, Guid AlertId)
    : IRequest<CameraMotionAlertDto>;

public record GetUnresolvedCameraMotionAlertsQuery(Guid CameraMarkId)
    : IRequest<List<CameraMotionAlertListItemDto>>;

public record GetCameraMotionAlertsInDateRangeQuery(Guid CameraMarkId, DateTime StartDate, DateTime EndDate)
    : IRequest<List<CameraMotionAlertListItemDto>>;

using Mapper.Application.Features.DTOs;
using MediatR;

namespace Mapper.Application.Features.CameraArchive.Queries;

public record GetCameraStatusHistoryQuery(Guid CameraMarkId, int Skip = 0, int Take = 50)
    : IRequest<List<CameraStatusHistoryListItemDto>>;

public record GetCameraStatusHistoryInDateRangeQuery(Guid CameraMarkId, DateTime StartDate, DateTime EndDate)
    : IRequest<List<CameraStatusHistoryListItemDto>>;

public record GetCameraCurrentStatusQuery(Guid CameraMarkId)
    : IRequest<CameraStatusHistoryDto?>;

using Mapper.Application.Features.DTOs;
using MediatR;

namespace Mapper.Application.Features.CameraArchive.Queries;

public record GetCameraVideoArchiveQuery(Guid CameraMarkId, int Skip = 0, int Take = 50) 
    : IRequest<List<CameraVideoArchiveListItemDto>>;

public record GetCameraVideoArchiveByIdQuery(Guid CameraMarkId, Guid VideoArchiveId)
    : IRequest<CameraVideoArchiveDto>;

public record GetCameraVideoArchiveTimelineQuery(Guid CameraMarkId, DateTime StartDate, DateTime EndDate)
    : IRequest<List<CameraVideoArchiveListItemDto>>;

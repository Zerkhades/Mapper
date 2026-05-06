using Mapper.Application.Features.Analytics.DTOs;
using MediatR;

namespace Mapper.Application.Features.Analytics.Queries;

public record GetMapGraphAnalyticsQuery(
    Guid? SourceGeoMapId = null,
    Guid? TargetGeoMapId = null,
    int Top = 5)
    : IRequest<MapGraphAnalyticsDto>;

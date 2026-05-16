using Mapper.Application.Features.Routes.DTOs;

namespace Mapper.WebApi.Models.Routes;

public record CreateRouteNodeRequest(
    double X,
    double Y,
    string? Title,
    Guid? GeoMarkId);

public record UpdateRouteNodeRequest(
    double X,
    double Y,
    string? Title,
    Guid? GeoMarkId);

public record CreateRouteEdgeRequest(
    Guid FromNodeId,
    Guid ToNodeId,
    bool IsBidirectional = true,
    double? CostOverride = null,
    string? Description = null);

public record UpdateRouteEdgeRequest(
    bool IsBidirectional,
    double? CostOverride,
    bool IsDisabled,
    string? Description);

public record CalculateRouteRequest(
    RouteEndpointDto Start,
    RouteEndpointDto End,
    double WalkingSpeed = 1.4);

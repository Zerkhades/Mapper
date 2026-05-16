namespace Mapper.Application.Features.Routes.DTOs;

public record RouteNodeDto(
    Guid Id,
    Guid GeoMapId,
    Guid? GeoMarkId,
    double X,
    double Y,
    string? Title);

public record RouteEdgeDto(
    Guid Id,
    Guid GeoMapId,
    Guid FromNodeId,
    Guid ToNodeId,
    double? CostOverride,
    bool IsBidirectional,
    bool IsDisabled,
    string? Description);

public record RoutePointDto(double X, double Y);

public record RouteEndpointDto(
    Guid? NodeId = null,
    Guid? GeoMarkId = null,
    double? X = null,
    double? Y = null);

public record RouteSegmentDto(
    Guid GeoMapId,
    IReadOnlyList<RoutePointDto> Points,
    double Distance,
    int EstimatedSeconds);

public record CalculatedRouteDto(
    IReadOnlyList<RouteSegmentDto> Segments,
    double TotalDistance,
    int EstimatedSeconds);

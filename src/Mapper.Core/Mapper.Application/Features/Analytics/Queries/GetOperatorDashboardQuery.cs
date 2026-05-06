using Mapper.Application.Features.Analytics.DTOs;
using MediatR;

namespace Mapper.Application.Features.Analytics.Queries;

public record GetOperatorDashboardQuery(
    DateTimeOffset? From = null,
    DateTimeOffset? To = null,
    int Top = 5)
    : IRequest<OperatorDashboardDto>;

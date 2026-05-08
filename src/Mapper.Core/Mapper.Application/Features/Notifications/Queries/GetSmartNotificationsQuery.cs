using Mapper.Application.Features.Notifications.DTOs;
using MediatR;

namespace Mapper.Application.Features.Notifications.Queries;

public record GetSmartNotificationsQuery(
    DateTimeOffset? From = null,
    DateTimeOffset? To = null,
    int OfflineGraceMinutes = 5,
    int Top = 50)
    : IRequest<List<SmartNotificationDto>>;

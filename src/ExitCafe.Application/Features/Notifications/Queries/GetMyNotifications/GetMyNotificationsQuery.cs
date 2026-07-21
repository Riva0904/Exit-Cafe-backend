using ExitCafe.Application.Features.Notifications.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Notifications.Queries.GetMyNotifications;

public record GetMyNotificationsQuery : IRequest<List<NotificationDto>>;

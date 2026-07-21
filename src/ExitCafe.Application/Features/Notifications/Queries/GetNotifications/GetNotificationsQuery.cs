using ExitCafe.Application.Common.Models;
using ExitCafe.Application.Features.Notifications.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Notifications.Queries.GetNotifications;

public class GetNotificationsQuery : PaginationParams, IRequest<PagedResult<NotificationDto>>
{
}

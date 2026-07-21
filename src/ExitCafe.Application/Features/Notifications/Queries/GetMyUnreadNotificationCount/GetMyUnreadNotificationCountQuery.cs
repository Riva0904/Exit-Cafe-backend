using MediatR;

namespace ExitCafe.Application.Features.Notifications.Queries.GetMyUnreadNotificationCount;

public record GetMyUnreadNotificationCountQuery : IRequest<int>;

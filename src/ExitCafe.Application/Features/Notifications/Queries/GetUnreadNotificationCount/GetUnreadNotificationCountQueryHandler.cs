using ExitCafe.Application.Common.Interfaces;
using MediatR;

namespace ExitCafe.Application.Features.Notifications.Queries.GetUnreadNotificationCount;

public class GetUnreadNotificationCountQueryHandler : IRequestHandler<GetUnreadNotificationCountQuery, int>
{
    private readonly IUnitOfWork _uow;

    public GetUnreadNotificationCountQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public Task<int> Handle(GetUnreadNotificationCountQuery request, CancellationToken ct) =>
        _uow.Notifications.CountAsync(n => !n.IsRead && n.CustomerId == null, ct);
}

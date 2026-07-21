using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Notifications.Queries.GetMyUnreadNotificationCount;

public class GetMyUnreadNotificationCountQueryHandler : IRequestHandler<GetMyUnreadNotificationCountQuery, int>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetMyUnreadNotificationCountQueryHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(GetMyUnreadNotificationCountQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException();
        var customer = await _uow.Customers.FirstOrDefaultAsync(c => c.UserId == userId, ct);
        if (customer is null) return 0;

        return await _uow.Notifications.CountAsync(n => !n.IsRead && n.CustomerId == customer.Id, ct);
    }
}

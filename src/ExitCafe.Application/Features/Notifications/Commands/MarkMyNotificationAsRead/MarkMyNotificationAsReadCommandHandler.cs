using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Notifications.Commands.MarkMyNotificationAsRead;

public class MarkMyNotificationAsReadCommandHandler : IRequestHandler<MarkMyNotificationAsReadCommand>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public MarkMyNotificationAsReadCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task Handle(MarkMyNotificationAsReadCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException();
        var customer = await _uow.Customers.FirstOrDefaultAsync(c => c.UserId == userId, ct)
            ?? throw new ForbiddenException("Your account has no customer profile.");

        var notification = await _uow.Notifications.Query()
            .FirstOrDefaultAsync(n => n.Id == request.Id && n.CustomerId == customer.Id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Notification), request.Id);

        notification.IsRead = true;
        _uow.Notifications.Update(notification);
        await _uow.SaveChangesAsync(ct);
    }
}

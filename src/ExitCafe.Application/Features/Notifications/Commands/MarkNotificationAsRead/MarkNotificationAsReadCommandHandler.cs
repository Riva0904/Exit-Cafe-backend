using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Domain.Exceptions;
using MediatR;

namespace ExitCafe.Application.Features.Notifications.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand>
{
    private readonly IUnitOfWork _uow;

    public MarkNotificationAsReadCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(MarkNotificationAsReadCommand request, CancellationToken ct)
    {
        var notification = await _uow.Notifications.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Notification), request.Id);

        notification.IsRead = true;
        _uow.Notifications.Update(notification);
        await _uow.SaveChangesAsync(ct);
    }
}

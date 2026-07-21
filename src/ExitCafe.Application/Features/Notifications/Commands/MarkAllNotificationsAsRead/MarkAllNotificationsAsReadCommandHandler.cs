using ExitCafe.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead;

public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand>
{
    private readonly IUnitOfWork _uow;

    public MarkAllNotificationsAsReadCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(MarkAllNotificationsAsReadCommand request, CancellationToken ct)
    {
        var unread = await _uow.Notifications.Query().Where(n => !n.IsRead && n.CustomerId == null).ToListAsync(ct);
        foreach (var n in unread) n.IsRead = true;
        await _uow.SaveChangesAsync(ct);
    }
}

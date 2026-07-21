using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Features.Orders.Events;
using ExitCafe.Domain.Entities;
using MediatR;

namespace ExitCafe.Application.Features.Notifications.EventHandlers;

public class OrderPlacedEventHandler : INotificationHandler<OrderPlacedEvent>
{
    private readonly IUnitOfWork _uow;

    public OrderPlacedEventHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(OrderPlacedEvent notification, CancellationToken ct)
    {
        await _uow.Notifications.AddAsync(new Notification
        {
            Title = "New order received",
            Message = $"Order {notification.OrderNumber} placed by {notification.CustomerFirstName} {notification.CustomerLastName} — ₹{notification.TotalAmount:0}",
            Type = "NewOrder",
            RelatedEntityId = notification.OrderId,
            IsRead = false,
        }, ct);
        await _uow.SaveChangesAsync(ct);
    }
}

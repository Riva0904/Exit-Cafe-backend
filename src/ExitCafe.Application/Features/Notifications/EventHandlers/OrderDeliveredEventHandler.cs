using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Features.Orders.Events;
using ExitCafe.Domain.Entities;
using MediatR;

namespace ExitCafe.Application.Features.Notifications.EventHandlers;

public class OrderDeliveredEventHandler : INotificationHandler<OrderDeliveredEvent>
{
    private readonly IUnitOfWork _uow;

    public OrderDeliveredEventHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(OrderDeliveredEvent notification, CancellationToken ct)
    {
        await _uow.Notifications.AddAsync(new Notification
        {
            Title = "Order delivered — rate your order",
            Message = $"Your order {notification.OrderNumber} has been delivered. Let us know how it was!",
            Type = "RateOrder",
            RelatedEntityId = notification.OrderId,
            CustomerId = notification.CustomerId,
            IsRead = false,
        }, ct);
        await _uow.SaveChangesAsync(ct);
    }
}

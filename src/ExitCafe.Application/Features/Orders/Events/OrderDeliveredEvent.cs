using MediatR;

namespace ExitCafe.Application.Features.Orders.Events;

public record OrderDeliveredEvent(Guid OrderId, string OrderNumber, Guid CustomerId) : INotification;

using MediatR;

namespace ExitCafe.Application.Features.Orders.Events;

public record OrderPlacedEvent(Guid OrderId, string OrderNumber, string CustomerFirstName, string CustomerLastName, decimal TotalAmount) : INotification;

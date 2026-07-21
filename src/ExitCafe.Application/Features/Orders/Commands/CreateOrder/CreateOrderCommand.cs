using ExitCafe.Application.Features.Orders.Dtos;
using ExitCafe.Domain.Enums;
using MediatR;

namespace ExitCafe.Application.Features.Orders.Commands.CreateOrder;

public record OrderItemRequest(Guid ProductId, int Quantity);

public record CreateOrderCommand(
    OrderType OrderType,
    Guid? DeliveryAddressId,
    string? DeliveryAddressLine1,
    string? DeliveryCity,
    string? DeliveryState,
    string? DeliveryPostalCode,
    DateOnly? DeliveryDate,
    TimeOnly? DeliveryTime,
    PaymentMethod PaymentMethod,
    string? CouponCode,
    string? Notes,
    List<OrderItemRequest> Items) : IRequest<OrderDto>;

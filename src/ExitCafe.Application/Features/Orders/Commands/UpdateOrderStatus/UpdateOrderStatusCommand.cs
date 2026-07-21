using ExitCafe.Application.Features.Orders.Dtos;
using ExitCafe.Domain.Enums;
using MediatR;

namespace ExitCafe.Application.Features.Orders.Commands.UpdateOrderStatus;

public record UpdateOrderStatusCommand(Guid Id, OrderStatus Status) : IRequest<OrderDto>;

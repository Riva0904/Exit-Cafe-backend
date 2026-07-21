using ExitCafe.Application.Features.Orders.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid Id) : IRequest<OrderDto>;

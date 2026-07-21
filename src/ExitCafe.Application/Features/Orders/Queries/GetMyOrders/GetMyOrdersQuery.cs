using ExitCafe.Application.Features.Orders.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Orders.Queries.GetMyOrders;

public record GetMyOrdersQuery : IRequest<List<OrderDto>>;

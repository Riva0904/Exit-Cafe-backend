using ExitCafe.Application.Features.Orders.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Orders.Queries.GetOrdersByCustomer;

public record GetOrdersByCustomerQuery(Guid CustomerId) : IRequest<List<OrderDto>>;

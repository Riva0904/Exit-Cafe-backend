using ExitCafe.Application.Common.Models;
using ExitCafe.Application.Features.Orders.Dtos;
using ExitCafe.Domain.Enums;
using MediatR;

namespace ExitCafe.Application.Features.Orders.Queries.GetOrders;

public class GetOrdersQuery : PaginationParams, IRequest<PagedResult<OrderDto>>
{
    public OrderStatus? Status { get; set; }
}

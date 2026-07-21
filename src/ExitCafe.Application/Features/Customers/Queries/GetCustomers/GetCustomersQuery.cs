using ExitCafe.Application.Common.Models;
using ExitCafe.Application.Features.Customers.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Customers.Queries.GetCustomers;

public class GetCustomersQuery : PaginationParams, IRequest<PagedResult<CustomerDto>>
{
}

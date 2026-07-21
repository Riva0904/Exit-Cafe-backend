using ExitCafe.Application.Features.Customers.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Customers.Queries.GetMyCustomer;

public record GetMyCustomerQuery : IRequest<CustomerDto>;

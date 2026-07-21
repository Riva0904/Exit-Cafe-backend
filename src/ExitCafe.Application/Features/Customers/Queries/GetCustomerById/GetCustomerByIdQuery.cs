using ExitCafe.Application.Features.Customers.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Customers.Queries.GetCustomerById;

public record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerDto>;

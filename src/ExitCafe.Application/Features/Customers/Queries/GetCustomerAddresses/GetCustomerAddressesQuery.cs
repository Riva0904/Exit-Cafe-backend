using ExitCafe.Application.Features.Customers.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Customers.Queries.GetCustomerAddresses;

public record GetCustomerAddressesQuery(Guid CustomerId) : IRequest<List<CustomerAddressDto>>;

using MediatR;

namespace ExitCafe.Application.Features.Customers.Commands.DeleteCustomerAddress;

public record DeleteCustomerAddressCommand(Guid CustomerId, Guid AddressId) : IRequest;

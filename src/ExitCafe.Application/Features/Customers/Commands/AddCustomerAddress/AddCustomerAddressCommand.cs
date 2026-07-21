using ExitCafe.Application.Features.Customers.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Customers.Commands.AddCustomerAddress;

public record AddCustomerAddressCommand(
    Guid CustomerId,
    string Label,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string State,
    string PostalCode,
    string Country,
    bool IsDefault) : IRequest<CustomerAddressDto>;

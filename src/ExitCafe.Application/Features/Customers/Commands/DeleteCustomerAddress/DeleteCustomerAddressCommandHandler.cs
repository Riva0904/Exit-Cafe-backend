using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Customers.Commands.DeleteCustomerAddress;

public class DeleteCustomerAddressCommandHandler : IRequestHandler<DeleteCustomerAddressCommand>
{
    private readonly IUnitOfWork _uow;

    public DeleteCustomerAddressCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(DeleteCustomerAddressCommand request, CancellationToken ct)
    {
        var address = await _uow.CustomerAddresses.Query()
            .FirstOrDefaultAsync(a => a.Id == request.AddressId && a.CustomerId == request.CustomerId, ct)
            ?? throw new NotFoundException(nameof(CustomerAddress), request.AddressId);

        _uow.CustomerAddresses.Remove(address);
        await _uow.SaveChangesAsync(ct);
    }
}

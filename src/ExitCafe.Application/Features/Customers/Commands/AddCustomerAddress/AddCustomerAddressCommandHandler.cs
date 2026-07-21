using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Features.Customers.Dtos;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Exceptions;
using MediatR;

namespace ExitCafe.Application.Features.Customers.Commands.AddCustomerAddress;

public class AddCustomerAddressCommandHandler : IRequestHandler<AddCustomerAddressCommand, CustomerAddressDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public AddCustomerAddressCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<CustomerAddressDto> Handle(AddCustomerAddressCommand request, CancellationToken ct)
    {
        if (!await _uow.Customers.AnyAsync(c => c.Id == request.CustomerId, ct))
            throw new NotFoundException(nameof(Customer), request.CustomerId);

        var address = new CustomerAddress
        {
            CustomerId = request.CustomerId,
            Label = request.Label,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            Country = request.Country,
            IsDefault = request.IsDefault
        };

        await _uow.CustomerAddresses.AddAsync(address, ct);
        await _uow.SaveChangesAsync(ct);
        return _mapper.Map<CustomerAddressDto>(address);
    }
}

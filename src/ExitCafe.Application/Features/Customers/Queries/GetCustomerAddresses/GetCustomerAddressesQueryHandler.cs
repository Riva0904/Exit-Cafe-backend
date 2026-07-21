using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Features.Customers.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Customers.Queries.GetCustomerAddresses;

public class GetCustomerAddressesQueryHandler : IRequestHandler<GetCustomerAddressesQuery, List<CustomerAddressDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetCustomerAddressesQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<List<CustomerAddressDto>> Handle(GetCustomerAddressesQuery request, CancellationToken ct)
    {
        var addresses = await _uow.CustomerAddresses.Query()
            .Where(a => a.CustomerId == request.CustomerId).ToListAsync(ct);
        return _mapper.Map<List<CustomerAddressDto>>(addresses);
    }
}

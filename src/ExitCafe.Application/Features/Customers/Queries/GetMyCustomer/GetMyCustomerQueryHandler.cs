using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Features.Customers.Dtos;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Customers.Queries.GetMyCustomer;

public class GetMyCustomerQueryHandler : IRequestHandler<GetMyCustomerQuery, CustomerDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public GetMyCustomerQueryHandler(IUnitOfWork uow, ICurrentUserService currentUser, IMapper mapper)
    {
        _uow = uow;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<CustomerDto> Handle(GetMyCustomerQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException();

        var customer = await _uow.Customers.Query().Include(c => c.Addresses).Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct)
            ?? throw new NotFoundException(nameof(Customer), userId);
        return _mapper.Map<CustomerDto>(customer);
    }
}

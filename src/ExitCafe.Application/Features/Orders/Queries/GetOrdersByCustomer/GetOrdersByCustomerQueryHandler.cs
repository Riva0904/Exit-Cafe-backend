using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Features.Orders.Dtos;
using ExitCafe.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Orders.Queries.GetOrdersByCustomer;

public class GetOrdersByCustomerQueryHandler : IRequestHandler<GetOrdersByCustomerQuery, List<OrderDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetOrdersByCustomerQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<List<OrderDto>> Handle(GetOrdersByCustomerQuery request, CancellationToken ct)
    {
        var orders = await _uow.Orders.Query().Include(o => o.Customer).Include(o => o.OrderItems).ThenInclude(i => i.Product)
            .Where(o => o.CustomerId == request.CustomerId)
            .OrderByDescending(o => o.CreatedAt).ToListAsync(ct);
        return _mapper.Map<List<OrderDto>>(orders);
    }
}

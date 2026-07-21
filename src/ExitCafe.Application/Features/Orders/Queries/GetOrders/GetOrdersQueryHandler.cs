using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Common.Models;
using ExitCafe.Application.Features.Orders.Dtos;
using ExitCafe.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Orders.Queries.GetOrders;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PagedResult<OrderDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetOrdersQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<PagedResult<OrderDto>> Handle(GetOrdersQuery request, CancellationToken ct)
    {
        IQueryable<Order> q = _uow.Orders.Query().Include(o => o.Customer).Include(o => o.OrderItems).ThenInclude(i => i.Product);
        if (request.Status.HasValue) q = q.Where(o => o.Status == request.Status);

        var totalCount = await q.CountAsync(ct);
        var items = await q.OrderByDescending(o => o.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync(ct);

        return new PagedResult<OrderDto>
        {
            Items = _mapper.Map<List<OrderDto>>(items),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}

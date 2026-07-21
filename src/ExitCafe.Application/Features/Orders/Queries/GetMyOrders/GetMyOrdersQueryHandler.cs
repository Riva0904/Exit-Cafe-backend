using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Features.Orders.Dtos;
using ExitCafe.Application.Features.Orders.Queries.GetOrdersByCustomer;
using ExitCafe.Domain.Exceptions;
using MediatR;

namespace ExitCafe.Application.Features.Orders.Queries.GetMyOrders;

public class GetMyOrdersQueryHandler : IRequestHandler<GetMyOrdersQuery, List<OrderDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public GetMyOrdersQueryHandler(IUnitOfWork uow, ICurrentUserService currentUser, IMediator mediator)
    {
        _uow = uow;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<List<OrderDto>> Handle(GetMyOrdersQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException();
        var customer = await _uow.Customers.FirstOrDefaultAsync(c => c.UserId == userId, ct);
        if (customer is null) return new List<OrderDto>();

        return await _mediator.Send(new GetOrdersByCustomerQuery(customer.Id), ct);
    }
}

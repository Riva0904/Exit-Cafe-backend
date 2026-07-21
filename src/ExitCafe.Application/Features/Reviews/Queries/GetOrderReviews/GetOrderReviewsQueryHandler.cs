using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Reviews.Queries.GetOrderReviews;

public class GetOrderReviewsQueryHandler : IRequestHandler<GetOrderReviewsQuery, List<Guid>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetOrderReviewsQueryHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<List<Guid>> Handle(GetOrderReviewsQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException();
        var customer = await _uow.Customers.FirstOrDefaultAsync(c => c.UserId == userId, ct);
        if (customer is null) return new List<Guid>();

        return await _uow.Reviews.Query()
            .Where(r => r.CustomerId == customer.Id && r.OrderId == request.OrderId)
            .Select(r => r.ProductId)
            .ToListAsync(ct);
    }
}

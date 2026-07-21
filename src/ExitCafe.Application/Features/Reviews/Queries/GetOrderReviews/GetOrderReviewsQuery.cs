using MediatR;

namespace ExitCafe.Application.Features.Reviews.Queries.GetOrderReviews;

public record GetOrderReviewsQuery(Guid OrderId) : IRequest<List<Guid>>;

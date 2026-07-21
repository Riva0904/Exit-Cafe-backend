using ExitCafe.Application.Features.Reviews.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Reviews.Commands.CreateReview;

public record CreateReviewCommand(Guid ProductId, Guid OrderId, int Rating, string? Comment) : IRequest<ReviewDto>;

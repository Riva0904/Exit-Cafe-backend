using ExitCafe.Application.Common.Models;
using ExitCafe.Application.Features.Reviews.Commands.CreateReview;
using ExitCafe.Application.Features.Reviews.Dtos;
using ExitCafe.Application.Features.Reviews.Queries.GetOrderReviews;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExitCafe.WebApi.Controllers;

[ApiController]
[Route("api/reviews")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ReviewDto>>> Create(CreateReviewCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(ApiResponse<ReviewDto>.Ok(result, "Thanks for rating your order!"));
    }

    [HttpGet("order/{orderId:guid}")]
    public async Task<ActionResult<ApiResponse<List<Guid>>>> GetOrderReviews(Guid orderId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetOrderReviewsQuery(orderId), ct);
        return Ok(ApiResponse<List<Guid>>.Ok(result));
    }
}

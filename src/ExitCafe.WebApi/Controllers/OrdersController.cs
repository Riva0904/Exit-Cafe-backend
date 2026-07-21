using ExitCafe.Application.Common.Models;
using ExitCafe.Application.Features.Orders.Commands.CancelOrder;
using ExitCafe.Application.Features.Orders.Commands.CreateOrder;
using ExitCafe.Application.Features.Orders.Commands.UpdateOrderStatus;
using ExitCafe.Application.Features.Orders.Dtos;
using ExitCafe.Application.Features.Orders.Queries.GetMyOrders;
using ExitCafe.Application.Features.Orders.Queries.GetOrderById;
using ExitCafe.Application.Features.Orders.Queries.GetOrders;
using ExitCafe.Application.Features.Orders.Queries.GetOrdersByCustomer;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExitCafe.WebApi.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<OrderDto>>>> GetMyOrders(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMyOrdersQuery(), ct);
        return Ok(ApiResponse<List<OrderDto>>.Ok(result));
    }

    [HttpGet]
    [Authorize(Policy = "StaffAndAbove")]
    public async Task<ActionResult<ApiResponse<PagedResult<OrderDto>>>> GetAll([FromQuery] GetOrdersQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        return Ok(ApiResponse<PagedResult<OrderDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id), ct);
        return Ok(ApiResponse<OrderDto>.Ok(result));
    }

    [HttpGet("customer/{customerId:guid}")]
    [Authorize(Policy = "StaffAndAbove")]
    public async Task<ActionResult<ApiResponse<List<OrderDto>>>> GetByCustomer(Guid customerId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetOrdersByCustomerQuery(customerId), ct);
        return Ok(ApiResponse<List<OrderDto>>.Ok(result));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<OrderDto>>> Create(CreateOrderCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<OrderDto>.Ok(result, "Order placed successfully."));
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "StaffAndAbove")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> UpdateStatus(Guid id, UpdateOrderStatusCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command with { Id = id }, ct);
        return Ok(ApiResponse<OrderDto>.Ok(result, "Order status updated."));
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new CancelOrderCommand(id), ct);
        return NoContent();
    }
}

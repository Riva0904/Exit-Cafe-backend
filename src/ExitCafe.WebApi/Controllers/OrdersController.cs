using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Common.Models;
using ExitCafe.Application.DTOs.Orders;
using ExitCafe.Application.Services.Interfaces;
using ExitCafe.Domain.Enums;
using ExitCafe.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExitCafe.WebApi.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ICurrentUserService _currentUserService;

    public OrdersController(IOrderService orderService, ICurrentUserService currentUserService)
    {
        _orderService = orderService;
        _currentUserService = currentUserService;
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<OrderDto>>>> GetMyOrders(CancellationToken ct)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedException();
        var result = await _orderService.GetByUserIdAsync(userId, ct);
        return Ok(ApiResponse<List<OrderDto>>.Ok(result));
    }

    [HttpGet]
    [Authorize(Policy = "StaffAndAbove")]
    public async Task<ActionResult<ApiResponse<PagedResult<OrderDto>>>> GetAll(
        [FromQuery] PaginationParams query, [FromQuery] OrderStatus? status, CancellationToken ct)
    {
        var result = await _orderService.GetAllAsync(query, status, ct);
        return Ok(ApiResponse<PagedResult<OrderDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _orderService.GetByIdAsync(id, ct);
        return Ok(ApiResponse<OrderDto>.Ok(result));
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<ActionResult<ApiResponse<List<OrderDto>>>> GetByCustomer(Guid customerId, CancellationToken ct)
    {
        var result = await _orderService.GetByCustomerAsync(customerId, ct);
        return Ok(ApiResponse<List<OrderDto>>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrderDto>>> Create(CreateOrderRequest request, CancellationToken ct)
    {
        var result = await _orderService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<OrderDto>.Ok(result, "Order placed successfully."));
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "StaffAndAbove")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> UpdateStatus(Guid id, UpdateOrderStatusRequest request, CancellationToken ct)
    {
        var result = await _orderService.UpdateStatusAsync(id, request.Status, ct);
        return Ok(ApiResponse<OrderDto>.Ok(result, "Order status updated."));
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        await _orderService.CancelAsync(id, ct);
        return NoContent();
    }
}

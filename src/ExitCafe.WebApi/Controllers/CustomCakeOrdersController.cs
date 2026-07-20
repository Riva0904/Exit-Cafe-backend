using ExitCafe.Application.Common.Models;
using ExitCafe.Application.DTOs.CustomCakeOrders;
using ExitCafe.Application.Services.Interfaces;
using ExitCafe.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExitCafe.WebApi.Controllers;

[ApiController]
[Route("api/custom-cake-orders")]
public class CustomCakeOrdersController : ControllerBase
{
    private readonly ICustomCakeOrderService _service;

    public CustomCakeOrdersController(ICustomCakeOrderService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CustomCakeOrderDto>>> Create(CreateCustomCakeOrderRequest request, CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<CustomCakeOrderDto>.Ok(result, "Your custom cake request has been submitted. We'll be in touch shortly!"));
    }

    [HttpGet]
    [Authorize(Policy = "StaffAndAbove")]
    public async Task<ActionResult<ApiResponse<PagedResult<CustomCakeOrderDto>>>> GetAll(
        [FromQuery] PaginationParams query, [FromQuery] CustomCakeOrderStatus? status, CancellationToken ct)
    {
        var result = await _service.GetAllAsync(query, status, ct);
        return Ok(ApiResponse<PagedResult<CustomCakeOrderDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "StaffAndAbove")]
    public async Task<ActionResult<ApiResponse<CustomCakeOrderDto>>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return Ok(ApiResponse<CustomCakeOrderDto>.Ok(result));
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "StaffAndAbove")]
    public async Task<ActionResult<ApiResponse<CustomCakeOrderDto>>> UpdateStatus(Guid id, UpdateCustomCakeOrderStatusRequest request, CancellationToken ct)
    {
        var result = await _service.UpdateStatusAsync(id, request.Status, ct);
        return Ok(ApiResponse<CustomCakeOrderDto>.Ok(result, "Status updated."));
    }
}

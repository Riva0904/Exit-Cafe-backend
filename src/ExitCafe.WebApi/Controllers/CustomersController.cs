using ExitCafe.Application.Common.Models;
using ExitCafe.Application.DTOs.Customers;
using ExitCafe.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExitCafe.WebApi.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    [Authorize(Policy = "StaffAndAbove")]
    public async Task<ActionResult<ApiResponse<PagedResult<CustomerDto>>>> GetAll([FromQuery] PaginationParams query, CancellationToken ct)
    {
        var result = await _customerService.GetAllAsync(query, ct);
        return Ok(ApiResponse<PagedResult<CustomerDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<CustomerDto>>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _customerService.GetByIdAsync(id, ct);
        return Ok(ApiResponse<CustomerDto>.Ok(result));
    }

    [HttpGet("{id:guid}/addresses")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<CustomerAddressDto>>>> GetAddresses(Guid id, CancellationToken ct)
    {
        var result = await _customerService.GetAddressesAsync(id, ct);
        return Ok(ApiResponse<List<CustomerAddressDto>>.Ok(result));
    }

    [HttpPost("{id:guid}/addresses")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<CustomerAddressDto>>> AddAddress(Guid id, CreateAddressRequest request, CancellationToken ct)
    {
        var result = await _customerService.AddAddressAsync(id, request, ct);
        return CreatedAtAction(nameof(GetAddresses), new { id }, ApiResponse<CustomerAddressDto>.Ok(result, "Address added."));
    }

    [HttpDelete("{id:guid}/addresses/{addressId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteAddress(Guid id, Guid addressId, CancellationToken ct)
    {
        await _customerService.DeleteAddressAsync(id, addressId, ct);
        return NoContent();
    }
}

using ExitCafe.Application.Common.Models;
using ExitCafe.Application.Features.Customers.Commands.AddCustomerAddress;
using ExitCafe.Application.Features.Customers.Commands.DeleteCustomerAddress;
using ExitCafe.Application.Features.Customers.Dtos;
using ExitCafe.Application.Features.Customers.Queries.GetCustomerAddresses;
using ExitCafe.Application.Features.Customers.Queries.GetCustomerById;
using ExitCafe.Application.Features.Customers.Queries.GetCustomers;
using ExitCafe.Application.Features.Customers.Queries.GetMyCustomer;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExitCafe.WebApi.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "StaffAndAbove")]
    public async Task<ActionResult<ApiResponse<PagedResult<CustomerDto>>>> GetAll([FromQuery] GetCustomersQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        return Ok(ApiResponse<PagedResult<CustomerDto>>.Ok(result));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<CustomerDto>>> GetMe(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMyCustomerQuery(), ct);
        return Ok(ApiResponse<CustomerDto>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<CustomerDto>>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCustomerByIdQuery(id), ct);
        return Ok(ApiResponse<CustomerDto>.Ok(result));
    }

    [HttpGet("{id:guid}/addresses")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<CustomerAddressDto>>>> GetAddresses(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCustomerAddressesQuery(id), ct);
        return Ok(ApiResponse<List<CustomerAddressDto>>.Ok(result));
    }

    [HttpPost("{id:guid}/addresses")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<CustomerAddressDto>>> AddAddress(Guid id, AddCustomerAddressCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command with { CustomerId = id }, ct);
        return CreatedAtAction(nameof(GetAddresses), new { id }, ApiResponse<CustomerAddressDto>.Ok(result, "Address added."));
    }

    [HttpDelete("{id:guid}/addresses/{addressId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteAddress(Guid id, Guid addressId, CancellationToken ct)
    {
        await _mediator.Send(new DeleteCustomerAddressCommand(id, addressId), ct);
        return NoContent();
    }
}

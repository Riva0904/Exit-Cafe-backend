using ExitCafe.Application.Common.Models;
using ExitCafe.Application.DTOs.Contact;
using ExitCafe.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExitCafe.WebApi.Controllers;

[ApiController]
[Route("api/contact")]
public class ContactController : ControllerBase
{
    private readonly IContactService _service;

    public ContactController(IContactService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ContactMessageDto>>> Create(CreateContactMessageRequest request, CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);
        return Ok(ApiResponse<ContactMessageDto>.Ok(result, "Thanks for reaching out — we'll get back to you soon."));
    }

    [HttpGet]
    [Authorize(Policy = "StaffAndAbove")]
    public async Task<ActionResult<ApiResponse<PagedResult<ContactMessageDto>>>> GetAll([FromQuery] PaginationParams query, CancellationToken ct)
    {
        var result = await _service.GetAllAsync(query, ct);
        return Ok(ApiResponse<PagedResult<ContactMessageDto>>.Ok(result));
    }

    [HttpPatch("{id:guid}/read")]
    [Authorize(Policy = "StaffAndAbove")]
    public async Task<ActionResult<ApiResponse<ContactMessageDto>>> MarkAsRead(Guid id, CancellationToken ct)
    {
        var result = await _service.MarkAsReadAsync(id, ct);
        return Ok(ApiResponse<ContactMessageDto>.Ok(result, "Marked as read."));
    }
}

using ExitCafe.Application.Common.Models;
using ExitCafe.Application.Features.Contact.Commands.CreateContactMessage;
using ExitCafe.Application.Features.Contact.Commands.MarkContactMessageAsRead;
using ExitCafe.Application.Features.Contact.Dtos;
using ExitCafe.Application.Features.Contact.Queries.GetContactMessages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExitCafe.WebApi.Controllers;

[ApiController]
[Route("api/contact")]
public class ContactController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContactController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ContactMessageDto>>> Create(CreateContactMessageCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(ApiResponse<ContactMessageDto>.Ok(result, "Thanks for reaching out — we'll get back to you soon."));
    }

    [HttpGet]
    [Authorize(Policy = "StaffAndAbove")]
    public async Task<ActionResult<ApiResponse<PagedResult<ContactMessageDto>>>> GetAll([FromQuery] GetContactMessagesQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        return Ok(ApiResponse<PagedResult<ContactMessageDto>>.Ok(result));
    }

    [HttpPatch("{id:guid}/read")]
    [Authorize(Policy = "StaffAndAbove")]
    public async Task<ActionResult<ApiResponse<ContactMessageDto>>> MarkAsRead(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new MarkContactMessageAsReadCommand(id), ct);
        return Ok(ApiResponse<ContactMessageDto>.Ok(result, "Marked as read."));
    }
}

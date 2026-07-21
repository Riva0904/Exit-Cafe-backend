using ExitCafe.Application.Common.Models;
using ExitCafe.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead;
using ExitCafe.Application.Features.Notifications.Commands.MarkNotificationAsRead;
using ExitCafe.Application.Features.Notifications.Dtos;
using ExitCafe.Application.Features.Notifications.Queries.GetNotifications;
using ExitCafe.Application.Features.Notifications.Queries.GetUnreadNotificationCount;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExitCafe.WebApi.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize(Policy = "StaffAndAbove")]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<NotificationDto>>>> GetAll([FromQuery] GetNotificationsQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        return Ok(ApiResponse<PagedResult<NotificationDto>>.Ok(result));
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<ApiResponse<int>>> GetUnreadCount(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUnreadNotificationCountQuery(), ct);
        return Ok(ApiResponse<int>.Ok(result));
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new MarkNotificationAsReadCommand(id), ct);
        return NoContent();
    }

    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken ct)
    {
        await _mediator.Send(new MarkAllNotificationsAsReadCommand(), ct);
        return NoContent();
    }
}

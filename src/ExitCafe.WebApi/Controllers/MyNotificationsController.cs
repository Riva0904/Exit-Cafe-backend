using ExitCafe.Application.Common.Models;
using ExitCafe.Application.Features.Notifications.Commands.MarkMyNotificationAsRead;
using ExitCafe.Application.Features.Notifications.Dtos;
using ExitCafe.Application.Features.Notifications.Queries.GetMyNotifications;
using ExitCafe.Application.Features.Notifications.Queries.GetMyUnreadNotificationCount;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExitCafe.WebApi.Controllers;

[ApiController]
[Route("api/notifications/my")]
[Authorize]
public class MyNotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MyNotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<NotificationDto>>>> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMyNotificationsQuery(), ct);
        return Ok(ApiResponse<List<NotificationDto>>.Ok(result));
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<ApiResponse<int>>> GetUnreadCount(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMyUnreadNotificationCountQuery(), ct);
        return Ok(ApiResponse<int>.Ok(result));
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new MarkMyNotificationAsReadCommand(id), ct);
        return NoContent();
    }
}

using ExitCafe.Application.Common.Models;
using ExitCafe.Application.DTOs.Notifications;
using ExitCafe.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExitCafe.WebApi.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize(Policy = "StaffAndAbove")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;

    public NotificationsController(INotificationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<NotificationDto>>>> GetAll([FromQuery] PaginationParams query, CancellationToken ct)
    {
        var result = await _service.GetAllAsync(query, ct);
        return Ok(ApiResponse<PagedResult<NotificationDto>>.Ok(result));
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<ApiResponse<int>>> GetUnreadCount(CancellationToken ct)
    {
        var result = await _service.GetUnreadCountAsync(ct);
        return Ok(ApiResponse<int>.Ok(result));
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken ct)
    {
        await _service.MarkAsReadAsync(id, ct);
        return NoContent();
    }

    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken ct)
    {
        await _service.MarkAllAsReadAsync(ct);
        return NoContent();
    }
}

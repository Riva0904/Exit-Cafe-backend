using ExitCafe.Application.Common.Models;
using ExitCafe.Application.DTOs.Notifications;

namespace ExitCafe.Application.Services.Interfaces;

public interface INotificationService
{
    Task CreateAsync(string title, string message, string type, Guid? relatedEntityId = null, CancellationToken ct = default);
    Task<PagedResult<NotificationDto>> GetAllAsync(PaginationParams query, CancellationToken ct = default);
    Task<int> GetUnreadCountAsync(CancellationToken ct = default);
    Task MarkAsReadAsync(Guid id, CancellationToken ct = default);
    Task MarkAllAsReadAsync(CancellationToken ct = default);
}

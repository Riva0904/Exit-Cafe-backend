using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Common.Models;
using ExitCafe.Application.DTOs.Notifications;
using ExitCafe.Application.Services.Interfaces;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public NotificationService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task CreateAsync(string title, string message, string type, Guid? relatedEntityId = null, CancellationToken ct = default)
    {
        await _uow.Notifications.AddAsync(new Notification
        {
            Title = title,
            Message = message,
            Type = type,
            RelatedEntityId = relatedEntityId,
            IsRead = false,
        }, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<PagedResult<NotificationDto>> GetAllAsync(PaginationParams query, CancellationToken ct = default)
    {
        var q = _uow.Notifications.Query();
        var totalCount = await q.CountAsync(ct);
        var items = await q.OrderByDescending(n => n.CreatedAt)
            .Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);

        return new PagedResult<NotificationDto>
        {
            Items = _mapper.Map<List<NotificationDto>>(items),
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public Task<int> GetUnreadCountAsync(CancellationToken ct = default) =>
        _uow.Notifications.CountAsync(n => !n.IsRead, ct);

    public async Task MarkAsReadAsync(Guid id, CancellationToken ct = default)
    {
        var notification = await _uow.Notifications.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Notification), id);

        notification.IsRead = true;
        _uow.Notifications.Update(notification);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task MarkAllAsReadAsync(CancellationToken ct = default)
    {
        var unread = await _uow.Notifications.Query().Where(n => !n.IsRead).ToListAsync(ct);
        foreach (var n in unread) n.IsRead = true;
        await _uow.SaveChangesAsync(ct);
    }
}

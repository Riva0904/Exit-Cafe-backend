using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Domain.Entities;

namespace ExitCafe.Infrastructure.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public AuditLogService(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task LogAsync(string action, string entityName, string? entityId, string? changes = null, CancellationToken ct = default)
    {
        var log = new AuditLog
        {
            UserId = _currentUserService.UserId,
            UserEmail = _currentUserService.Email,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            Changes = changes,
            IpAddress = _currentUserService.IpAddress
        };

        await _uow.AuditLogs.AddAsync(log, ct);
        await _uow.SaveChangesAsync(ct);
    }
}

using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Enums;
using ExitCafe.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand>
{
    private static readonly HashSet<string> StaffRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "SuperAdmin", "Admin", "Manager", "Staff",
    };

    private readonly IUnitOfWork _uow;
    private readonly IAuditLogService _auditLog;
    private readonly ICurrentUserService _currentUser;

    public CancelOrderCommandHandler(IUnitOfWork uow, IAuditLogService auditLog, ICurrentUserService currentUser)
    {
        _uow = uow;
        _auditLog = auditLog;
        _currentUser = currentUser;
    }

    public async Task Handle(CancelOrderCommand request, CancellationToken ct)
    {
        var order = await _uow.Orders.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Order), request.Id);

        var userId = _currentUser.UserId ?? throw new UnauthorizedException();
        var isStaff = _currentUser.Role is not null && StaffRoles.Contains(_currentUser.Role);

        if (!isStaff)
        {
            var customer = await _uow.Customers.FirstOrDefaultAsync(c => c.UserId == userId, ct)
                ?? throw new ForbiddenException("This order does not belong to you.");
            if (order.CustomerId != customer.Id)
                throw new ForbiddenException("This order does not belong to you.");
        }

        if (order.Status is OrderStatus.Delivered or OrderStatus.OutForDelivery or OrderStatus.Cancelled)
            throw new BadRequestException($"Order in status '{order.Status}' cannot be cancelled.");

        order.Status = OrderStatus.Cancelled;
        // See UpdateOrderStatusCommandHandler: no explicit Update() — order is already tracked from
        // GetByIdAsync, and calling Update() here would wipe OrderItems (loaded as an empty default
        // collection, not included) on this Cascade-configured relationship.
        await _uow.SaveChangesAsync(ct);
        await _auditLog.LogAsync("OrderCancelled", nameof(Order), order.Id.ToString(), ct: ct);
    }
}

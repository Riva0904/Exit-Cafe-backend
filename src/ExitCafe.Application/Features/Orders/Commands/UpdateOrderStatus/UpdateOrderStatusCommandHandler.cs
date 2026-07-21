using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Features.Orders.Dtos;
using ExitCafe.Application.Features.Orders.Events;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Enums;
using ExitCafe.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, OrderDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IAuditLogService _auditLog;
    private readonly IPublisher _publisher;

    public UpdateOrderStatusCommandHandler(IUnitOfWork uow, IMapper mapper, IAuditLogService auditLog, IPublisher publisher)
    {
        _uow = uow;
        _mapper = mapper;
        _auditLog = auditLog;
        _publisher = publisher;
    }

    public async Task<OrderDto> Handle(UpdateOrderStatusCommand request, CancellationToken ct)
    {
        var order = await _uow.Orders.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Order), request.Id);

        ValidateStatusTransition(order.Status, request.Status);
        order.Status = request.Status;

        _uow.Orders.Update(order);
        await _uow.SaveChangesAsync(ct);
        await _auditLog.LogAsync("OrderStatusChanged", nameof(Order), order.Id.ToString(), $"-> {request.Status}", ct);

        if (request.Status == OrderStatus.Delivered)
            await _publisher.Publish(new OrderDeliveredEvent(order.Id, order.OrderNumber, order.CustomerId), ct);

        var updated = await _uow.Orders.Query().Include(o => o.Customer).Include(o => o.OrderItems).ThenInclude(i => i.Product)
            .FirstAsync(o => o.Id == request.Id, ct);
        return _mapper.Map<OrderDto>(updated);
    }

    private static void ValidateStatusTransition(OrderStatus current, OrderStatus next)
    {
        if (current == OrderStatus.Cancelled || current == OrderStatus.Delivered)
            throw new BadRequestException($"Order in status '{current}' cannot be changed.");
    }
}

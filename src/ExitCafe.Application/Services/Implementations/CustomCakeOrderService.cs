using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Common.Models;
using ExitCafe.Application.DTOs.CustomCakeOrders;
using ExitCafe.Application.Services.Interfaces;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Enums;
using ExitCafe.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Services.Implementations;

public class CustomCakeOrderService : ICustomCakeOrderService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IAuditLogService _auditLog;

    public CustomCakeOrderService(IUnitOfWork uow, IMapper mapper, IAuditLogService auditLog)
    {
        _uow = uow;
        _mapper = mapper;
        _auditLog = auditLog;
    }

    public async Task<CustomCakeOrderDto> CreateAsync(CreateCustomCakeOrderRequest request, CancellationToken ct = default)
    {
        TimeOnly? deliveryTime = null;
        if (!string.IsNullOrWhiteSpace(request.DeliveryTime) && TimeOnly.TryParse(request.DeliveryTime, out var parsed))
            deliveryTime = parsed;

        var order = new CustomCakeOrder
        {
            CustomerName = request.CustomerName,
            Email = request.Email,
            Phone = request.Phone,
            Occasion = request.Occasion,
            Size = request.Size,
            Flavor = request.Flavor,
            Shape = request.Shape,
            ThemeColor = request.ThemeColor,
            Toppings = request.Toppings is { Count: > 0 } ? string.Join(", ", request.Toppings) : null,
            CakeMessage = request.CakeMessage,
            DeliveryDate = request.DeliveryDate,
            DeliveryTime = deliveryTime,
            Budget = request.Budget,
            Notes = request.Notes,
            ReferenceImageUrl = request.ReferenceImageUrl,
            Status = CustomCakeOrderStatus.New,
        };

        await _uow.CustomCakeOrders.AddAsync(order, ct);
        await _uow.SaveChangesAsync(ct);
        await _auditLog.LogAsync("CustomCakeOrderCreated", nameof(CustomCakeOrder), order.Id.ToString(), ct: ct);

        return _mapper.Map<CustomCakeOrderDto>(order);
    }

    public async Task<PagedResult<CustomCakeOrderDto>> GetAllAsync(PaginationParams query, CustomCakeOrderStatus? status, CancellationToken ct = default)
    {
        IQueryable<CustomCakeOrder> q = _uow.CustomCakeOrders.Query();
        if (status.HasValue) q = q.Where(o => o.Status == status);

        var totalCount = await q.CountAsync(ct);
        var items = await q.OrderByDescending(o => o.CreatedAt)
            .Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);

        return new PagedResult<CustomCakeOrderDto>
        {
            Items = _mapper.Map<List<CustomCakeOrderDto>>(items),
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<CustomCakeOrderDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var order = await _uow.CustomCakeOrders.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(CustomCakeOrder), id);
        return _mapper.Map<CustomCakeOrderDto>(order);
    }

    public async Task<CustomCakeOrderDto> UpdateStatusAsync(Guid id, CustomCakeOrderStatus status, CancellationToken ct = default)
    {
        var order = await _uow.CustomCakeOrders.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(CustomCakeOrder), id);

        order.Status = status;
        _uow.CustomCakeOrders.Update(order);
        await _uow.SaveChangesAsync(ct);
        await _auditLog.LogAsync("CustomCakeOrderStatusChanged", nameof(CustomCakeOrder), order.Id.ToString(), $"-> {status}", ct);

        return _mapper.Map<CustomCakeOrderDto>(order);
    }
}

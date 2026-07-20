using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Common.Models;
using ExitCafe.Application.DTOs.Orders;
using ExitCafe.Application.Services.Interfaces;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Enums;
using ExitCafe.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Services.Implementations;

public class OrderService : IOrderService
{
    private const decimal TaxRate = 0.05m;
    private const decimal DeliveryFeeFlat = 40m;

    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IAuditLogService _auditLog;

    public OrderService(IUnitOfWork uow, IMapper mapper, IAuditLogService auditLog)
    {
        _uow = uow;
        _mapper = mapper;
        _auditLog = auditLog;
    }

    private IQueryable<Order> BaseQuery() =>
        _uow.Orders.Query().Include(o => o.Customer).Include(o => o.OrderItems).ThenInclude(i => i.Product);

    public async Task<PagedResult<OrderDto>> GetAllAsync(PaginationParams query, OrderStatus? status, CancellationToken ct = default)
    {
        var q = BaseQuery();
        if (status.HasValue) q = q.Where(o => o.Status == status);

        var totalCount = await q.CountAsync(ct);
        var items = await q.OrderByDescending(o => o.CreatedAt)
            .Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);

        return new PagedResult<OrderDto>
        {
            Items = _mapper.Map<List<OrderDto>>(items),
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<List<OrderDto>> GetByCustomerAsync(Guid customerId, CancellationToken ct = default) =>
        _mapper.Map<List<OrderDto>>(await BaseQuery().Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt).ToListAsync(ct));

    public async Task<List<OrderDto>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var customer = await _uow.Customers.FirstOrDefaultAsync(c => c.UserId == userId, ct);
        if (customer is null) return new List<OrderDto>();

        return await GetByCustomerAsync(customer.Id, ct);
    }

    public async Task<OrderDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var order = await BaseQuery().FirstOrDefaultAsync(o => o.Id == id, ct)
            ?? throw new NotFoundException(nameof(Order), id);
        return _mapper.Map<OrderDto>(order);
    }

    public async Task<OrderDto> CreateAsync(CreateOrderRequest request, CancellationToken ct = default)
    {
        if (request.Items.Count == 0)
            throw new BadRequestException("Order must contain at least one item.");

        var customerId = request.CustomerId;
        if (customerId is null)
        {
            if (string.IsNullOrWhiteSpace(request.GuestEmail))
                throw new BadRequestException("Guest email is required for guest checkout.");

            var guest = new Customer
            {
                FirstName = request.GuestFirstName ?? "Guest",
                LastName = request.GuestLastName ?? "",
                Email = request.GuestEmail,
                Phone = request.GuestPhone,
                IsGuest = true
            };
            await _uow.Customers.AddAsync(guest, ct);
            await _uow.SaveChangesAsync(ct);
            customerId = guest.Id;
        }
        else if (!await _uow.Customers.AnyAsync(c => c.Id == customerId, ct))
        {
            throw new NotFoundException(nameof(Customer), customerId);
        }

        var orderItems = new List<OrderItem>();
        decimal subTotal = 0;

        foreach (var item in request.Items)
        {
            var product = await _uow.Products.GetByIdAsync(item.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), item.ProductId);

            if (!product.IsAvailable)
                throw new BadRequestException($"Product '{product.Name}' is not available.");

            var unitPrice = product.DiscountPrice ?? product.Price;
            var totalPrice = unitPrice * item.Quantity;
            subTotal += totalPrice;

            orderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = unitPrice,
                Quantity = item.Quantity,
                TotalPrice = totalPrice
            });
        }

        var deliveryFee = request.OrderType == OrderType.Delivery ? DeliveryFeeFlat : 0m;
        var taxAmount = Math.Round(subTotal * TaxRate, 2);
        var discountAmount = 0m;
        var totalAmount = subTotal + taxAmount + deliveryFee - discountAmount;

        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            CustomerId = customerId.Value,
            Status = OrderStatus.Pending,
            OrderType = request.OrderType,
            SubTotal = subTotal,
            DiscountAmount = discountAmount,
            TaxAmount = taxAmount,
            DeliveryFee = deliveryFee,
            TotalAmount = totalAmount,
            CouponCode = request.CouponCode,
            DeliveryAddressId = request.DeliveryAddressId,
            DeliveryAddressLine1 = request.DeliveryAddressLine1,
            DeliveryCity = request.DeliveryCity,
            DeliveryState = request.DeliveryState,
            DeliveryPostalCode = request.DeliveryPostalCode,
            DeliveryDate = request.DeliveryDate,
            DeliveryTime = request.DeliveryTime,
            PaymentMethod = request.PaymentMethod,
            PaymentStatus = PaymentStatus.Pending,
            Notes = request.Notes,
            OrderItems = orderItems
        };

        await _uow.Orders.AddAsync(order, ct);
        await _uow.SaveChangesAsync(ct);
        await _auditLog.LogAsync("OrderCreated", nameof(Order), order.Id.ToString(), ct: ct);

        var created = await BaseQuery().FirstAsync(o => o.Id == order.Id, ct);
        return _mapper.Map<OrderDto>(created);
    }

    public async Task<OrderDto> UpdateStatusAsync(Guid id, OrderStatus status, CancellationToken ct = default)
    {
        var order = await _uow.Orders.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Order), id);

        ValidateStatusTransition(order.Status, status);
        order.Status = status;

        _uow.Orders.Update(order);
        await _uow.SaveChangesAsync(ct);
        await _auditLog.LogAsync("OrderStatusChanged", nameof(Order), order.Id.ToString(), $"-> {status}", ct);

        var updated = await BaseQuery().FirstAsync(o => o.Id == id, ct);
        return _mapper.Map<OrderDto>(updated);
    }

    public async Task CancelAsync(Guid id, CancellationToken ct = default)
    {
        var order = await _uow.Orders.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Order), id);

        if (order.Status is OrderStatus.Delivered or OrderStatus.OutForDelivery)
            throw new BadRequestException("Cannot cancel an order that is already out for delivery or delivered.");

        order.Status = OrderStatus.Cancelled;
        _uow.Orders.Update(order);
        await _uow.SaveChangesAsync(ct);
        await _auditLog.LogAsync("OrderCancelled", nameof(Order), order.Id.ToString(), ct: ct);
    }

    private static void ValidateStatusTransition(OrderStatus current, OrderStatus next)
    {
        if (current == OrderStatus.Cancelled || current == OrderStatus.Delivered)
            throw new BadRequestException($"Order in status '{current}' cannot be changed.");
    }

    private static string GenerateOrderNumber() =>
        $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpperInvariant()}";
}

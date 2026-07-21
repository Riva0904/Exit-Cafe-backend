using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Features.Orders.Dtos;
using ExitCafe.Application.Features.Orders.Events;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Enums;
using ExitCafe.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private const decimal TaxRate = 0.05m;
    private const decimal DeliveryFeeFlat = 40m;

    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IAuditLogService _auditLog;
    private readonly IPublisher _publisher;
    private readonly ICurrentUserService _currentUser;

    public CreateOrderCommandHandler(IUnitOfWork uow, IMapper mapper, IAuditLogService auditLog, IPublisher publisher, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _auditLog = auditLog;
        _publisher = publisher;
        _currentUser = currentUser;
    }

    private IQueryable<Order> BaseQuery() =>
        _uow.Orders.Query().Include(o => o.Customer).Include(o => o.OrderItems).ThenInclude(i => i.Product);

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("You must be signed in to place an order.");
        var customer = await _uow.Customers.FirstOrDefaultAsync(c => c.UserId == userId, ct)
            ?? throw new NotFoundException(nameof(Customer), userId);
        var customerId = customer.Id;

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
            CustomerId = customerId,
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

        await _publisher.Publish(
            new OrderPlacedEvent(order.Id, order.OrderNumber, created.Customer.FirstName, created.Customer.LastName, order.TotalAmount),
            ct);

        return _mapper.Map<OrderDto>(created);
    }

    private static string GenerateOrderNumber() =>
        $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpperInvariant()}";
}

using ExitCafe.Domain.Enums;

namespace ExitCafe.Application.DTOs.Orders;

public record OrderItemRequest(Guid ProductId, int Quantity);

public record CreateOrderRequest(
    Guid? CustomerId,
    string? GuestFirstName, string? GuestLastName, string? GuestEmail, string? GuestPhone,
    OrderType OrderType,
    Guid? DeliveryAddressId,
    string? DeliveryAddressLine1,
    string? DeliveryCity,
    string? DeliveryState,
    string? DeliveryPostalCode,
    DateOnly? DeliveryDate,
    TimeOnly? DeliveryTime,
    PaymentMethod PaymentMethod,
    string? CouponCode,
    string? Notes,
    List<OrderItemRequest> Items);

public class OrderItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}

public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = default!;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = default!;
    public OrderStatus Status { get; set; }
    public OrderType OrderType { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal TotalAmount { get; set; }
    public string? CouponCode { get; set; }
    public string? DeliveryAddressLine1 { get; set; }
    public string? DeliveryCity { get; set; }
    public string? DeliveryState { get; set; }
    public string? DeliveryPostalCode { get; set; }
    public DateOnly? DeliveryDate { get; set; }
    public TimeOnly? DeliveryTime { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public record UpdateOrderStatusRequest(OrderStatus Status);

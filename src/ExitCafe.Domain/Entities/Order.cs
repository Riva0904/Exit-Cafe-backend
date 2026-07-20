using ExitCafe.Domain.Common;
using ExitCafe.Domain.Enums;

namespace ExitCafe.Domain.Entities;

public class Order : BaseAuditableEntity
{
    public string OrderNumber { get; set; } = default!;

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public OrderType OrderType { get; set; } = OrderType.Delivery;

    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal TotalAmount { get; set; }

    public string? CouponCode { get; set; }

    public Guid? DeliveryAddressId { get; set; }
    public CustomerAddress? DeliveryAddress { get; set; }
    public DateOnly? DeliveryDate { get; set; }
    public TimeOnly? DeliveryTime { get; set; }

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CashOnDelivery;

    public string? Notes { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

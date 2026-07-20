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

    // Denormalized delivery address for guest checkout, where there's no saved address book
    // entry to reference yet. DeliveryAddressId remains available for registered customers
    // ordering against a saved address.
    public string? DeliveryAddressLine1 { get; set; }
    public string? DeliveryCity { get; set; }
    public string? DeliveryState { get; set; }
    public string? DeliveryPostalCode { get; set; }

    public DateOnly? DeliveryDate { get; set; }
    public TimeOnly? DeliveryTime { get; set; }

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CashOnDelivery;

    public string? Notes { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

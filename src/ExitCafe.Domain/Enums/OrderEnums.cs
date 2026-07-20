namespace ExitCafe.Domain.Enums;

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Preparing = 2,
    Baking = 3,
    Ready = 4,
    OutForDelivery = 5,
    Delivered = 6,
    Cancelled = 7
}

public enum OrderType
{
    Delivery = 0,
    Pickup = 1
}

public enum PaymentStatus
{
    Pending = 0,
    Paid = 1,
    Failed = 2,
    Refunded = 3
}

public enum PaymentMethod
{
    CashOnDelivery = 0,
    Card = 1,
    UPI = 2,
    Wallet = 3
}

public enum DiscountType
{
    Percentage = 0,
    FixedAmount = 1
}

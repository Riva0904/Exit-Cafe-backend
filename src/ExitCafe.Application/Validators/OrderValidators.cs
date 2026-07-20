using ExitCafe.Application.DTOs.Orders;
using ExitCafe.Domain.Enums;
using FluentValidation;

namespace ExitCafe.Application.Validators;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.Items).NotEmpty().WithMessage("Order must contain at least one item.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty();
            item.RuleFor(i => i.Quantity).GreaterThan(0);
        });

        RuleFor(x => x.GuestEmail).EmailAddress().When(x => x.CustomerId == null)
            .WithMessage("A valid email is required for guest checkout.");

        RuleFor(x => x)
            .Must(x => x.DeliveryAddressId.HasValue || !string.IsNullOrWhiteSpace(x.DeliveryAddressLine1))
            .When(x => x.OrderType == OrderType.Delivery)
            .WithMessage("A delivery address is required for delivery orders.")
            .OverridePropertyName("DeliveryAddressLine1");

        RuleFor(x => x.DeliveryCity).NotEmpty()
            .When(x => x.OrderType == OrderType.Delivery && !x.DeliveryAddressId.HasValue)
            .WithMessage("City is required for delivery orders.");

        RuleFor(x => x.DeliveryState).NotEmpty()
            .When(x => x.OrderType == OrderType.Delivery && !x.DeliveryAddressId.HasValue)
            .WithMessage("State is required for delivery orders.");

        RuleFor(x => x.DeliveryPostalCode).NotEmpty()
            .When(x => x.OrderType == OrderType.Delivery && !x.DeliveryAddressId.HasValue)
            .WithMessage("Postal code is required for delivery orders.");
    }
}

public class UpdateOrderStatusRequestValidator : AbstractValidator<UpdateOrderStatusRequest>
{
    public UpdateOrderStatusRequestValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}

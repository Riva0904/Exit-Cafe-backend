using ExitCafe.Domain.Enums;
using FluentValidation;

namespace ExitCafe.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Items).NotEmpty().WithMessage("Order must contain at least one item.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty();
            item.RuleFor(i => i.Quantity).GreaterThan(0);
        });

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

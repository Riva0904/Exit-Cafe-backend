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

        RuleFor(x => x.DeliveryAddressId).NotEmpty()
            .When(x => x.OrderType == OrderType.Delivery)
            .WithMessage("Delivery address is required for delivery orders.");
    }
}

public class UpdateOrderStatusRequestValidator : AbstractValidator<UpdateOrderStatusRequest>
{
    public UpdateOrderStatusRequestValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}

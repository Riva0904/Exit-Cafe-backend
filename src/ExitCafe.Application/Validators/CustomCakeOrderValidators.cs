using ExitCafe.Application.DTOs.CustomCakeOrders;
using FluentValidation;

namespace ExitCafe.Application.Validators;

public class CreateCustomCakeOrderRequestValidator : AbstractValidator<CreateCustomCakeOrderRequest>
{
    public CreateCustomCakeOrderRequestValidator()
    {
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Phone).NotEmpty().MinimumLength(8).MaximumLength(20);
        RuleFor(x => x.Occasion).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Size).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Flavor).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DeliveryDate).GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.Date))
            .WithMessage("Delivery date must be today or later.");
        RuleFor(x => x.Budget).GreaterThan(0).When(x => x.Budget.HasValue);
        RuleFor(x => x.CakeMessage).MaximumLength(200);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}

public class UpdateCustomCakeOrderStatusRequestValidator : AbstractValidator<UpdateCustomCakeOrderStatusRequest>
{
    public UpdateCustomCakeOrderStatusRequestValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}

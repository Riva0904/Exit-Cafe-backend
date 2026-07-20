using ExitCafe.Application.DTOs.Products;
using FluentValidation;

namespace ExitCafe.Application.Validators;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.SKU).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.DiscountPrice).LessThan(x => x.Price)
            .When(x => x.DiscountPrice.HasValue)
            .WithMessage("Discount price must be less than the regular price.");
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.DiscountPrice).LessThan(x => x.Price)
            .When(x => x.DiscountPrice.HasValue)
            .WithMessage("Discount price must be less than the regular price.");
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}

public class UpdateProductImagesRequestValidator : AbstractValidator<UpdateProductImagesRequest>
{
    public UpdateProductImagesRequestValidator()
    {
        RuleFor(x => x.ImageUrls).NotEmpty().WithMessage("At least one image URL is required.");
        RuleForEach(x => x.ImageUrls).NotEmpty().Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Each image URL must be a valid absolute URL.");
    }
}

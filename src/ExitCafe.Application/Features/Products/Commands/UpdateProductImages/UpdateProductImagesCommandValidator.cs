using FluentValidation;

namespace ExitCafe.Application.Features.Products.Commands.UpdateProductImages;

public class UpdateProductImagesCommandValidator : AbstractValidator<UpdateProductImagesCommand>
{
    public UpdateProductImagesCommandValidator()
    {
        RuleFor(x => x.ImageUrls).NotEmpty().WithMessage("At least one image URL is required.");
        RuleForEach(x => x.ImageUrls).NotEmpty().Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Each image URL must be a valid absolute URL.");
    }
}

using FluentValidation;

namespace ExitCafe.Application.Features.Uploads.Commands.UploadImages;

public class UploadImagesCommandValidator : AbstractValidator<UploadImagesCommand>
{
    private static readonly HashSet<string> AllowedSubfolders = new(StringComparer.OrdinalIgnoreCase)
    {
        "products", "categories",
    };
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp", "image/gif",
    };
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    public UploadImagesCommandValidator()
    {
        RuleFor(x => x.Subfolder).Must(s => AllowedSubfolders.Contains(s))
            .WithMessage("Unknown upload target.");

        RuleFor(x => x.Files).NotEmpty().WithMessage("Select at least one image to upload.");

        RuleForEach(x => x.Files).ChildRules(file =>
        {
            file.RuleFor(f => f.ContentType).Must(ct => AllowedContentTypes.Contains(ct))
                .WithMessage("Only JPEG, PNG, WEBP and GIF images are allowed.");
            file.RuleFor(f => f.Length).LessThanOrEqualTo(MaxFileSizeBytes)
                .WithMessage("Each image must be 5MB or smaller.");
        });
    }
}

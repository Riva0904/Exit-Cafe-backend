using ExitCafe.Application.DTOs.Contact;
using FluentValidation;

namespace ExitCafe.Application.Validators;

public class CreateContactMessageRequestValidator : AbstractValidator<CreateContactMessageRequest>
{
    public CreateContactMessageRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Phone).MaximumLength(20);
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Message).NotEmpty().MaximumLength(4000);
    }
}

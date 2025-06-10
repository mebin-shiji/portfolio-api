using FluentValidation;

namespace portfolio_api.Features.Mail.Send;

public sealed class SendMailCommandValidator : AbstractValidator<SendMailCommand>
{
    public SendMailCommandValidator()
    {
        RuleFor(x => x.From)
            .MaximumLength(255)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.From))
            .WithMessage("Invalid email address format.");

        RuleFor(x => x.FromName)
            .MaximumLength(100);

        RuleFor(x => x.To)
            .MaximumLength(255)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.To))
            .WithMessage("Invalid email address format.");

        RuleFor(x => x.ToName)
            .MaximumLength(100);

        RuleFor(x => x.Subject)
            .NotEmpty()
            .WithMessage("Subject cannot be empty.")
            .MaximumLength(255);

        RuleFor(x => x.Body)
            .NotEmpty()
            .WithMessage("Body cannot be empty.");

        RuleForEach(x => x.Attachments)
            .NotNull()
            .WithMessage("Attachment cannot be null.")
            .SetValidator(new AttachmentValidator());

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.From) || !string.IsNullOrWhiteSpace(x.To))
            .WithMessage("Either 'From' or 'To' must be provided.");
    }
}

internal sealed class AttachmentValidator : AbstractValidator<Attachment>
{
    public AttachmentValidator()
    {
        RuleFor(x => x.Filename)
            .NotNull()
            .NotEmpty()
            .WithMessage("Filename cannot be empty.");

        RuleFor(x => x.Url)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("URL is required.")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out var _))
            .WithMessage("Invalid URL format.");
    }
}
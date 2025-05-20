using FluentValidation;

namespace portfolio_api.Infrastructure.Email;

internal sealed class EmailSettingsValidator : AbstractValidator<EmailSettings>
{
    public EmailSettingsValidator()
    {
        RuleFor(x => x.SmtpServer)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("SMTP server is required.")
            .MaximumLength(255)
            .WithMessage("SMTP server name cannot exceed 255 characters.");

        RuleFor(x => x.SmtpPort)
            .Cascade(CascadeMode.Stop)
            .InclusiveBetween(1, 65535)
            .WithMessage("SMTP port must be between 1 and 65535.")
            .Must(port => port == 25 || port == 465 || port == 587 || port == 2525)
            .WithMessage("Port {PropertyValue} is not a standard SMTP port. Common SMTP ports are 25, 465, 587, and 2525.");

        RuleFor(x => x.DefaultFromEmail)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("From email address is required.")
            .EmailAddress()
            .WithMessage("Invalid from email address format.")
            .MaximumLength(255)
            .WithMessage("Email address cannot exceed 255 characters.");

        RuleFor(x => x.DefaultFromName)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("From name is required.")
            .MaximumLength(100)
            .WithMessage("From name cannot exceed 100 characters.");

        RuleFor(x => x.DefaultToEmail)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("To email address is required.")
            .EmailAddress()
            .WithMessage("Invalid to email address format.")
            .MaximumLength(255)
            .WithMessage("Email address cannot exceed 255 characters.");

        RuleFor(x => x.DefaultToName)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("To name is required.")
            .MaximumLength(100)
            .WithMessage("To name cannot exceed 100 characters.");

        RuleFor(x => x.UserName)
            .MaximumLength(100)
            .WithMessage("Username cannot exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.UserName));

        RuleFor(x => x.Password)
            .MaximumLength(100)
            .WithMessage("Password cannot exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Password));

        RuleFor(x => x)
            .Must(x =>
                (string.IsNullOrWhiteSpace(x.UserName) && string.IsNullOrWhiteSpace(x.Password)) ||
                (!string.IsNullOrWhiteSpace(x.UserName) && !string.IsNullOrWhiteSpace(x.Password))
            )
            .WithMessage("Both Username and Password must be provided if either one is specified.");
    }
}
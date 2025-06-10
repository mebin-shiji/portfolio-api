using FluentValidation;
using portfolio_api.Domain.Enums;
using System.Net;

namespace portfolio_api.Features.AuditLog.Create;
internal sealed class CreateAuditLogCommandValidator : AbstractValidator<CreateAuditLogCommand>
{
    public CreateAuditLogCommandValidator()
    {
        RuleFor(x => x.EventType)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .Must(a => Enum.TryParse<EventType>(a, true, out _))
            .WithMessage("Action must be one of: Page, Email, Download.");

        RuleFor(x => x.Page)
            .MaximumLength(500)
            .WithMessage("Page cannot exceed 500 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.IpAddress)
            .NotNull()
            .NotEmpty()
            .WithMessage("IpAddress is required.");

        RuleFor(x => x.IpAddress)
            .Must(x => IPAddress.TryParse(x, out _))
            .WithMessage("IpAddress must be a valid IP address.");

        RuleFor(x => x.UserAgent)
            .MaximumLength(500)
            .WithMessage("UserAgent cannot exceed 500 characters.");

        RuleFor(x => x.CreatedAt)
           .LessThanOrEqualTo(DateTime.UtcNow)
           .WithMessage("CreatedAt cannot be in the future.");

        RuleFor(x => x.CreatedAt)
            .Must(x => x != default)
            .WithMessage("Please Provide a time stamp for CreatedAt.");
    }
}
using FluentValidation;
using portfolio_api.Domain.Enums;
using System.Net;

namespace portfolio_api.Features.AuditLog.Create;
public sealed class CreateAuditLogCommandValidator : AbstractValidator<CreateAuditLogCommand>
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
    }
}
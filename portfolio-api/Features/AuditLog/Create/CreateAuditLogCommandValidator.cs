using FluentValidation;
using System.Net;

namespace portfolio_api.Features.AuditLog.Create;
public class CreateAuditLogCommandValidator : AbstractValidator<CreateAuditLogCommand>
{
    public CreateAuditLogCommandValidator()
    {
        RuleFor(x => x.Action)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .Must(a => Enum.TryParse<ActionType>(a, true, out _))
            .WithMessage("Action must be one of: CREATE, DELETE, UPDATE, READ.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.CreatedAt)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("CreatedAt cannot be in the future.");

        RuleFor(x => x.CreatedAt)
            .Must(x => x != default)
            .WithMessage("Please Provide a time stamp for CreatedAt.");

        RuleFor(x => x.IpAddress)
            .NotNull()
            .NotEmpty()
            .WithMessage("IpAddress is required.");

        RuleFor(x => x.IpAddress)
            .Must(x => IPAddress.TryParse(x, out _))
            .WithMessage("IpAddress must be a valid IP address.");
    }
}
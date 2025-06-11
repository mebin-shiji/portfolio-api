using FluentValidation;

namespace portfolio_api.Features.Upload.CreateSasToken;

internal sealed class CreateSasTokenCommandValidator : AbstractValidator<CreateSasTokenCommand>
{
    public CreateSasTokenCommandValidator()
    {
        RuleFor(x => x.ContainerName)
            .MaximumLength(255)
            .WithMessage("Container name cannot exceed 255 characters.");
    }
}

using FluentValidation;

namespace portfolio_api.Features.Upload.CreateSasToken
{
    internal sealed class CreateSasTokenCommandValidator : AbstractValidator<CreateSasTokenCommand>
    {
        public CreateSasTokenCommandValidator()
        {
            RuleFor(x => x.BlobName)
                .NotEmpty()
                .WithMessage("Blob name cannot be empty.")
                .MaximumLength(255)
                .WithMessage("Blob name cannot exceed 255 characters.");

            RuleFor(x => x.ContainerName)
                .MaximumLength(255)
                .WithMessage("Container name cannot exceed 255 characters.");
        }
    }
}

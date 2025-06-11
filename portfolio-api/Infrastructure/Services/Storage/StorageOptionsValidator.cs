using FluentValidation;

namespace portfolio_api.Infrastructure.Services.Storage;

internal sealed class StorageOptionsValidator : AbstractValidator<StorageOptions>
{
    public StorageOptionsValidator() 
    { 
        RuleFor(x => x.AccountName)
            .NotNull()
            .NotEmpty()
            .WithMessage("Account name is required.");

        RuleFor(x => x.AccountKey)
            .NotNull()
            .NotEmpty()
            .WithMessage("Account key is required.");
    }
}

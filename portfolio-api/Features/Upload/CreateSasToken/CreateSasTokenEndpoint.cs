using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using portfolio_api.Infrastructure.Storage;

namespace portfolio_api.Features.Upload.CreateSasToken
{
    public static class CreateSasTokenEndpoint
    {
        private static readonly TimeSpan Expiry = TimeSpan.FromMinutes(15);
        public static void MapCreateSasToken(this WebApplication app)
        {
            app.MapPost("/upload/createSasToken", CreateSasToken).WithName("CreateSasToken").WithTags("Upload");
        }

        public static async Task<Results<Ok<CreateSasTokenResponse>, ProblemHttpResult, ValidationProblem>> CreateSasToken(CreateSasTokenCommand request, IValidator<CreateSasTokenCommand> validator, IAzureStorageService azureStorageService, HttpContext context, IConfiguration configuration)
        {
            var ct = context.RequestAborted;
            var validationResult = await validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var sasToken = azureStorageService.GenerateBlobSasToken(request.ContainerName ?? configuration["EmailAttachments"]!, request.BlobName, Expiry);
                return TypedResults.Ok(new CreateSasTokenResponse(sasToken));
            }
            catch (Exception)
            {
                return TypedResults.Problem("An error occurred while creating the SAS token.", statusCode: 500);
            }
        }

    }
}

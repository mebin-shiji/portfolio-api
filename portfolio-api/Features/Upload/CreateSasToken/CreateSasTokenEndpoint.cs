using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using portfolio_api.Infrastructure.Services.Storage;

namespace portfolio_api.Features.Upload.CreateSasToken;

public static class CreateSasTokenEndpoint
{
    private static readonly TimeSpan Expiry = TimeSpan.FromMinutes(15);
    public static void MapCreateSasToken(this WebApplication app)
    {
        app.MapGet("/upload/createSasToken", CreateSasToken).WithName("CreateSasToken").WithTags("Upload").RequireRateLimiting("UploadPolicy");
    }

    public static Results<Ok<CreateSasTokenResponse>, ProblemHttpResult, ValidationProblem> CreateSasToken([FromQuery] string containerName, [FromServices] IValidator<CreateSasTokenCommand> validator, [FromServices] IStorageService storageService, HttpContext context, IConfiguration configuration)
    {
        // Validate the container name
        Dictionary<string, string[]> errors = [];
        if (containerName is null)
        {
            errors.Add("ContainerName", [ "Container name is required." ]);
        }
        if (containerName?.Length > 255)
        {
            errors.Add("ContainerName", ["Container name cannot exceed 255 characters."]);
        }
        if(errors.Count > 0)
        {
            return TypedResults.ValidationProblem(errors);
        }

        try
        {
            var sasToken = storageService.GenerateContainerSasToken(containerName!, Expiry);
            return TypedResults.Ok(new CreateSasTokenResponse(sasToken));
        }
        catch (Exception)
        {
            return TypedResults.Problem("An error occurred while creating the SAS token.", statusCode: 500);
        }
    }

}

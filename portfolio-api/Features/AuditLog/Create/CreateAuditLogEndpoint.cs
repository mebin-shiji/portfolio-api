using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace portfolio_api.Features.AuditLog.Create;

public static class CreateAuditLogEndpoint
{
    public static void MapCreateAuditLog(this WebApplication app)
    {
        app.MapPost("/auditlog", CreateAuditLog).WithName("CreateAuditLog").WithTags("AuditLogs");
    }

    public static async Task<Results<Created, ProblemHttpResult, ValidationProblem>> CreateAuditLog([FromBody] CreateAuditLogCommand request, [FromServices] IValidator<CreateAuditLogCommand> validator, [FromServices] ICreateAuditLogHandler handler, HttpContext context, CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        try
        {
            await handler.Handle(request, context, ct);
            return TypedResults.Created();
        }
        catch (Exception)
        {
            return TypedResults.Problem("An error occurred while creating the audit log.", statusCode: 500);
        }
    }

}
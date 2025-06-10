using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using portfolio_api.Infrastructure.Services.Email;

namespace portfolio_api.Features.Mail.Send
{
    public static class SendMailEndpoint
    {
        public static void MapSendEmail(this WebApplication app)
        {
            app.MapPost("/mail", SendEmail).WithName("SendEmail").WithTags("SendEmail");
        }

        public static async Task<Results<Created, ProblemHttpResult, ValidationProblem>> SendEmail([FromBody] SendMailCommand request, [FromServices] IValidator<SendMailCommand> validator, [FromServices] IEmailService emailService, HttpContext context, CancellationToken ct)
        {
            var validationResult = await validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                await emailService.SendEmailAsync([request], ct);
                return TypedResults.Created();
            }
            catch (Exception)
            {
                return TypedResults.Problem("An error occurred while sending the mail.", statusCode: 500);
            }
        }
    }
}

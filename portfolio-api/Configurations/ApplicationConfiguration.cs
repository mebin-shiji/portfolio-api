using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using portfolio_api.Features.AuditLog.Create;
using portfolio_api.Features.Mail.Send;
using portfolio_api.Features.Upload.CreateSasToken;
using Scalar.AspNetCore;

namespace portfolio_api.Configurations;
public static class ApplicationConfiguration
{
    public static WebApplication AddApiEndpoints(this WebApplication app)
    {
        app.MapCreateSasToken();
        app.MapCreateAuditLog();
        app.MapSendEmail();

        app.MapHealthChecks("/_health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        return app;
    }

}

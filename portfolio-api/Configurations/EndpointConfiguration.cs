using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using portfolio_api.Features.AuditLog.Create;
using portfolio_api.Features.Mail.Send;
using portfolio_api.Features.Upload.CreateSasToken;

namespace portfolio_api.Configurations;
public static class EndpointConfiguration
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

        return app;
    }

}

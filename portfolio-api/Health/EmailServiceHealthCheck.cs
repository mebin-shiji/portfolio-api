using MailKit.Net.Smtp;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using portfolio_api.Infrastructure.Email;

namespace portfolio_api.Health
{
    public class EmailServiceHealthCheck(IOptions<EmailSettings> emailSettings, ILogger<EmailServiceHealthCheck> logger) : IHealthCheck
    {
        private readonly EmailSettings _emailSettings = emailSettings.Value ?? throw new ArgumentNullException(nameof(emailSettings), "Email settings options cannot be null.");
        private readonly ILogger<EmailServiceHealthCheck> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Checking SMTP server health...");

                using var client = new SmtpClient();

                // Set timeout to fail fast if unreachable
                client.Timeout = 5000;

                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, _emailSettings.EnableSsl, cancellationToken);

                if (!string.IsNullOrWhiteSpace(_emailSettings.UserName) && !string.IsNullOrWhiteSpace(_emailSettings.Password))
                {
                    await client.AuthenticateAsync(_emailSettings.UserName, _emailSettings.Password, cancellationToken);
                }

                await client.DisconnectAsync(true, cancellationToken);
                
                _logger.LogInformation("SMTP server is reachable.");

                return HealthCheckResult.Healthy("SMTP server is reachable");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SMTP server is not reachable. Health check failed with message: {Message}", ex.Message);
                return HealthCheckResult.Unhealthy("Failed to connect to SMTP server", ex);
            }
        }
    }
}

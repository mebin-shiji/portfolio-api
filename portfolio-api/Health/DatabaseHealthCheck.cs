using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using portfolio_api.Infrastructure.Persistance;

namespace portfolio_api.Health
{
    public class DatabaseHealthCheck(AppDbContext dbContext, ILogger<DatabaseHealthCheck> logger) : IHealthCheck
    {
        private readonly ILogger<DatabaseHealthCheck> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly AppDbContext _dbContext = dbContext;
        private const int TimeoutSeconds = 5;

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Checking PostgreSQL database health...");

                var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(TimeoutSeconds));

                // Test 1: Basic connection test
                var canConnect = await _dbContext.Database.CanConnectAsync(cts.Token);
                if (!canConnect)
                {
                    return HealthCheckResult.Unhealthy("Could not connect to PostgreSQL database");
                }
                _logger.LogInformation("Test 1: Basic connection test succeeded.");

                // Test 2: Execute a simple query
                // This checks if the database is actually responsive
                var result = await _dbContext.Database.ExecuteSqlRawAsync("SELECT 1", cts.Token);

                _logger.LogInformation("Test 2: PostgreSQL database query executed successfully.");

                return HealthCheckResult.Healthy("PostgreSQL database is healthy",
                    new Dictionary<string, object>
                    {
                    { "Response Time (ms)", DateTime.UtcNow.Millisecond }
                    });
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Database health check timed out after {TimeoutSeconds} seconds.", TimeoutSeconds);
                return HealthCheckResult.Degraded("Database health check timed out");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed with message: {Message}", ex.Message);
                return HealthCheckResult.Unhealthy("Database is not reachable", ex);
            }
        }
    }
}

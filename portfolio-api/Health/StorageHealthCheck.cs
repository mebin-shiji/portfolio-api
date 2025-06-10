using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using portfolio_api.Infrastructure.Services.Storage;

namespace portfolio_api.Health;

public class StorageHealthCheck(ILogger<StorageHealthCheck> logger, IOptions<StorageOptions> storageOptions) : IHealthCheck
{
    private readonly ILogger<StorageHealthCheck> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly StorageOptions _storageOptions = storageOptions?.Value ?? throw new ArgumentNullException(nameof(storageOptions), "Storage options cannot be null.");
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Simulate a storage health check
            await Task.Delay(100, cancellationToken); // Simulate async operation
            // If the operation succeeds, return healthy status
            return HealthCheckResult.Healthy("Storage service is healthy.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Storage health check failed.");
            return HealthCheckResult.Unhealthy("Storage service is unhealthy.", ex);
        }
    }
}

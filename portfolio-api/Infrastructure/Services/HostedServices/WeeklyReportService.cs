namespace portfolio_api.Infrastructure.Services.HostedServices;

public class WeeklyReportService : BackgroundService
{
    private readonly ILogger<WeeklyReportService> _logger;
    private readonly IServiceProvider _serviceProvider;
    public WeeklyReportService(IServiceProvider serviceProvider, ILogger<WeeklyReportService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("WeeklyReportService is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("WeeklyReportService is begining to perform work.");

            // Your logic to send weekly reports goes here
            await Task.Delay(TimeSpan.FromDays(7), stoppingToken);
        }
        _logger.LogInformation("WeeklyReportService is stopping.");

    }
}

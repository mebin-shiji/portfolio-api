using FluentValidation;
using Microsoft.EntityFrameworkCore;
using portfolio_api.Domain.Enums;
using portfolio_api.Features.AuditLog.Create;
using portfolio_api.Health;
using portfolio_api.Infrastructure.Persistance;
using portfolio_api.Infrastructure.Services.Email;
using portfolio_api.Infrastructure.Services.HostedServices;
using portfolio_api.Infrastructure.Services.Storage;
using System.Threading.RateLimiting;

namespace portfolio_api.Configurations;

public static class ServiceConfiguration
{
    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(optionsBuilder =>
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("NpgSqlConnection"),
            o => o.MapEnum<EventType>("event_type")),
            ServiceLifetime.Singleton
        );

        return services;
    }

    private static IServiceCollection AddApplicationHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("Database")
            .AddCheck<EmailServiceHealthCheck>("Email Service");

        return services;
    }

    private static IServiceCollection AddStorageService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<StorageOptions>()
           .Bind(configuration.GetSection("StorageOptions"))
           .Validate<IValidator<StorageOptions>>((options, validator) =>
           {
               var result = validator.Validate(options);
               return result.IsValid;
           })
           .ValidateOnStart();

        services.AddSingleton<IStorageService, AzureStorageService>();

        return services;
    }

    private static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<EmailSettings>()
            .Bind(configuration.GetSection("EmailSettings"))
            .Validate<IValidator<EmailSettings>>((settings, validator) =>
            {
                var result = validator.Validate(settings);
                return result.IsValid;
            })
            .ValidateOnStart();

        services.AddScoped<IEmailService, EmailService>();

        return services;
    }

    private static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.AddPolicy("AuditPolicy", httpContext =>
            
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                    factory: partitionKey => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 50,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 10
                    }
                )
            );

            options.AddPolicy("MailPolicy", httpContext =>

                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                    factory: partitionKey => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 10
                    }
                )
            );

            options.AddPolicy("UploadPolicy", httpContext =>

               RateLimitPartition.GetFixedWindowLimiter(
                   partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                   factory: partitionKey => new FixedWindowRateLimiterOptions
                   {
                       PermitLimit = 5,
                       Window = TimeSpan.FromMinutes(1),
                       QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                       QueueLimit = 10
                   }
               )
           );

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });

        return services;
    }

    private static IServiceCollection AddApplicationServices(this IServiceCollection services) => services.AddScoped<ICreateAuditLogHandler, CreateAuditLogHandler>();

    private static IServiceCollection AddHostedServices(this IServiceCollection services) => services.AddHostedService<WeeklyReportService>();

    private static IServiceCollection AddOpenAPISpecification(this IServiceCollection services) => services.AddOpenApi();

    private static IServiceCollection AddValidators(this IServiceCollection services) => services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);

    // Master method to configure all services
    public static IServiceCollection AddApplicationServiceConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration)
            .AddApplicationHealthChecks()
            .AddValidators()
            .AddStorageService(configuration)
            .AddEmailService(configuration)
            .AddApplicationServices()
            .AddHostedServices()
            .AddOpenAPISpecification()
            .AddRateLimiting();
        
        return services;
    }
}

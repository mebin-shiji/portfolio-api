using FluentValidation;
using Microsoft.EntityFrameworkCore;
using portfolio_api.Domain.Enums;
using portfolio_api.Features.AuditLog.Create;
using portfolio_api.Features.Mail.Send;
using portfolio_api.Features.Upload.CreateSasToken;
using portfolio_api.Health;
using portfolio_api.Infrastructure.Persistance;
using portfolio_api.Infrastructure.Services.Email;
using portfolio_api.Infrastructure.Services.HostedServices;
using portfolio_api.Infrastructure.Services.Storage;

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
        // Configure and validate options
        services.AddOptions<StorageOptions>()
           .Bind(configuration.GetSection("StorageOptions"))
           .Validate<IValidator<StorageOptions>>((options, validator) =>
           {
               var result = validator.Validate(options);
               return result.IsValid;
           })
           .ValidateOnStart();

        // Register service
        services.AddSingleton<IStorageService, AzureStorageService>();

        return services;
    }

    private static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure and validate options
        services.AddOptions<EmailSettings>()
            .Bind(configuration.GetSection("EmailSettings"))
            .Validate<IValidator<EmailSettings>>((settings, validator) =>
            {
                var result = validator.Validate(settings);
                return result.IsValid;
            })
            .ValidateOnStart();

        // Register service
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateAuditLogCommandValidator>();
        services.AddValidatorsFromAssemblyContaining<SendMailCommandValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateSasTokenCommandValidator>();
        services.AddSingleton<IValidator<StorageOptions>, StorageOptionsValidator>();
        services.AddSingleton<IValidator<EmailSettings>, EmailSettingsValidator>();

        return services;
    }

    private static IServiceCollection AddApplicationServices(this IServiceCollection services) => services.AddScoped<ICreateAuditLogHandler, CreateAuditLogHandler>();

    private static IServiceCollection AddHostedServices(this IServiceCollection services) => services.AddHostedService<WeeklyReportService>();

    private static IServiceCollection AddOpenAPISpecification(this IServiceCollection services) => services.AddOpenApi();

    // Master method to configure all services
    public static IServiceCollection AddApplicationServiceConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddApplicationHealthChecks();
        services.AddValidators();
        services.AddStorageService(configuration);
        services.AddEmailService(configuration);
        services.AddApplicationServices();
        services.AddHostedServices();
        services.AddOpenAPISpecification();

        return services;
    }
}

using FluentValidation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using portfolio_api.Features.AuditLog.Create;
using portfolio_api.Features.Mail.Send;
using portfolio_api.Features.Upload.CreateSasToken;
using portfolio_api.Health;
using portfolio_api.Infrastructure.Persistance;
using portfolio_api.Infrastructure.Services.Email;
using portfolio_api.Infrastructure.Services.HostedServices;
using portfolio_api.Infrastructure.Services.Storage;

namespace portfolio_api.Configurations
{
    public static class ServiceConfiguration
    {
        public static WebApplication AddApiEndpoints(this WebApplication app)
        {
            app.MapCreateSasToken();
            app.MapCreateAuditLog();

            app.MapHealthChecks("/_health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            return app;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseNpgsql(configuration.GetConnectionString("NpgSqlConnection"));
            });

            return services;
        }

        public static IServiceCollection AddApplicationHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>("Database")
                .AddCheck<EmailServiceHealthCheck>("Email Service");

            return services;
        }

        public static IServiceCollection AddStorageService(this IServiceCollection services, IConfiguration configuration)
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

        public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
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

        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CreateAuditLogCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<SendMailCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateSasTokenCommandValidator>();
            services.AddSingleton<IValidator<StorageOptions>, StorageOptionsValidator>();
            services.AddSingleton<IValidator<EmailSettings>, EmailSettingsValidator>();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ICreateAuditLogHandler, CreateAuditLogHandler>();

            return services;
        }

        public static IServiceCollection AddHostedServices(this IServiceCollection services)
        {
            services.AddHostedService<WeeklyReportService>();

            return services;
        }

        // Master method to configure all services
        public static IServiceCollection AddApplicationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatabase(configuration);
            services.AddApplicationHealthChecks();
            services.AddValidators();
            services.AddStorageService(configuration);
            services.AddEmailService(configuration);
            services.AddApplicationServices();
            services.AddHostedServices();

            return services;
        }
    }
}

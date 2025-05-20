using FluentValidation;
using Microsoft.EntityFrameworkCore;
using portfolio_api.Features.AuditLog.Create;
using portfolio_api.Features.Mail.Send;
using portfolio_api.Infrastructure.Email;
using portfolio_api.Infrastructure.HostedServices;
using portfolio_api.Infrastructure.Persistance;
using portfolio_api.Infrastructure.Storage;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/webapi.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Use Logging
builder.Host.UseSerilog();

// Register DB Context
builder.Services.AddDbContext<AppDbContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Register Options
builder.Services.AddOptions<EmailSettings>().Bind(builder.Configuration.GetSection("EmailSettings")).ValidateOnStart();
builder.Services.Configure<AzureStorageOptions>(builder.Configuration.GetSection("AzureBlobStorage"));

// Register Validators
builder.Services.AddValidatorsFromAssemblyContaining<CreateAuditLogCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<SendMailCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AzureOptionsValidator>();

// Register Services
builder.Services.AddSingleton<IAzureStorageService, AzureStorageService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICreateAuditLogHandler, CreateAuditLogHandler>();

// Register Hosted Services
builder.Services.AddHostedService<WeeklyReportService>();

var app = builder.Build();

// Map Endpoints
app.MapCreateAuditLog();

app.Run();

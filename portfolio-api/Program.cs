using portfolio_api.Configurations;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/webapi.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Use Serilog
builder.Host.UseSerilog();

// Add all application services
builder.Services.AddApplicationConfiguration(builder.Configuration);

var app = builder.Build();

// Add the endpoints
app.AddApiEndpoints();

app.Run();
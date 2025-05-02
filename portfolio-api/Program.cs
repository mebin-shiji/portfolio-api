using Microsoft.EntityFrameworkCore;
using portfolio_api.Infrastructure.Email;
using portfolio_api.Infrastructure.Persistance;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings")); 

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();

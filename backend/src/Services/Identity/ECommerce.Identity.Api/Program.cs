using ECommerce.Identity.Api.Endpoints;
using ECommerce.Identity.Api.ExceptionHandling;
using ECommerce.Identity.Infrastructure;
using ECommerce.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

const string FrontendCorsPolicy = "FrontendCorsPolicy";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<AuthenticationFailedExceptionHandler>();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddProblemDetails();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var connectionString = builder.Configuration.GetConnectionString("IdentityDb")
    ?? throw new InvalidOperationException("Connection string 'IdentityDb' is not configured.");

builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "identity-db");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    dbContext.Database.Migrate();

    await AdminSeeder.SeedAsync(scope.ServiceProvider);
}

app.UseExceptionHandler();

app.UseCors(FrontendCorsPolicy);

app.UseSwagger();
app.UseSwaggerUI();

app.MapHealthChecks("/health");

app.MapAuthEndpoints();

app.Run();

public partial class Program { }

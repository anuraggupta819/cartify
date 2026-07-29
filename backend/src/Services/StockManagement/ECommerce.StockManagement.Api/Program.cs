using System.Text;
using ECommerce.StockManagement.Api.Endpoints;
using ECommerce.StockManagement.Api.ExceptionHandling;
using ECommerce.StockManagement.Infrastructure;
using ECommerce.StockManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

const string RequireAdminRolePolicy = "RequireAdminRole";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStockManagementInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddProblemDetails();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Configuration 'Jwt:Key' is not set.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("Configuration 'Jwt:Issuer' is not set.");
var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("Configuration 'Jwt:Audience' is not set.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true,
            RoleClaimType = "role",
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(RequireAdminRolePolicy, policy => policy.RequireRole("Admin"));
});

var connectionString = builder.Configuration.GetConnectionString("StockManagementDb")
    ?? throw new InvalidOperationException("Connection string 'StockManagementDb' is not configured.");

builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "stock-management-db");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<StockManagementDbContext>();
    dbContext.Database.Migrate();
}

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapHealthChecks("/health").AllowAnonymous();

app.MapStockEndpoints();

app.Run();

public partial class Program { }

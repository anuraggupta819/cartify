using System.Text;
using ECommerce.PaymentProcessing.Api.Endpoints;
using ECommerce.PaymentProcessing.Api.ExceptionHandling;
using ECommerce.PaymentProcessing.Application.Options;
using ECommerce.PaymentProcessing.Infrastructure;
using ECommerce.PaymentProcessing.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPaymentProcessingInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var razorpayKeyId = builder.Configuration["Razorpay:KeyId"]
    ?? throw new InvalidOperationException("Configuration 'Razorpay:KeyId' is not set.");
builder.Services.AddSingleton(new RazorpayPublicSettings(razorpayKeyId));

builder.Services.AddExceptionHandler<InvalidOperationExceptionHandler>();
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

builder.Services.AddAuthorization();

var connectionString = builder.Configuration.GetConnectionString("PaymentProcessingDb")
    ?? throw new InvalidOperationException("Connection string 'PaymentProcessingDb' is not configured.");

builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "payment-processing-db");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PaymentProcessingDbContext>();
    dbContext.Database.Migrate();
}

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapHealthChecks("/health").AllowAnonymous();

app.MapPaymentEndpoints();

app.Run();

public partial class Program { }

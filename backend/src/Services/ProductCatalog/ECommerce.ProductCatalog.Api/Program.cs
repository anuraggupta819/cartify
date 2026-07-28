using ECommerce.ProductCatalog.Api.Endpoints;
using ECommerce.ProductCatalog.Api.ExceptionHandling;
using ECommerce.ProductCatalog.Infrastructure;
using ECommerce.ProductCatalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

const string FrontendCorsPolicy = "FrontendCorsPolicy";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProductCatalogInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var connectionString = builder.Configuration.GetConnectionString("ProductCatalogDb")
    ?? throw new InvalidOperationException("Connection string 'ProductCatalogDb' is not configured.");

builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "product-catalog-db");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ProductCatalogDbContext>();
    dbContext.Database.Migrate();
}

app.UseExceptionHandler();

app.UseCors(FrontendCorsPolicy);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");

app.MapProductEndpoints();
app.MapCategoryEndpoints();

app.Run();

public partial class Program { }

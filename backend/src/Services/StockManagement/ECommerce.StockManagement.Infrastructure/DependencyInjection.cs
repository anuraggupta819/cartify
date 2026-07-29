using ECommerce.StockManagement.Application.Abstractions;
using ECommerce.StockManagement.Application.Services;
using ECommerce.StockManagement.Infrastructure.Persistence;
using ECommerce.StockManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.StockManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddStockManagementInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("StockManagementDb")
            ?? throw new InvalidOperationException("Connection string 'StockManagementDb' is not configured.");

        services.AddDbContext<StockManagementDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IStockRepository, StockRepository>();
        services.AddScoped<StockService>();

        return services;
    }
}

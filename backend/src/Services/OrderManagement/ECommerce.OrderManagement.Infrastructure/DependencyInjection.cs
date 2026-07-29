using ECommerce.OrderManagement.Application.Abstractions;
using ECommerce.OrderManagement.Application.Services;
using ECommerce.OrderManagement.Infrastructure.BackgroundServices;
using ECommerce.OrderManagement.Infrastructure.ExternalServices;
using ECommerce.OrderManagement.Infrastructure.Persistence;
using ECommerce.OrderManagement.Infrastructure.Repositories;
using ECommerce.OrderManagement.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.OrderManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddOrderManagementInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("OrderManagementDb")
            ?? throw new InvalidOperationException("Connection string 'OrderManagementDb' is not configured.");

        services.AddDbContext<OrderManagementDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<OrderService>();

        services.AddHttpContextAccessor();
        services.AddScoped<SystemJwtIssuer>();
        services.AddScoped<OutboundAuthorization>();

        services.AddHttpClient<IProductCatalogClient, HttpProductCatalogClient>();
        services.AddHttpClient<IStockReservationClient, HttpStockReservationClient>();
        services.AddHttpClient<IStockFinalizationClient, HttpStockFinalizationClient>();

        services.AddHostedService<AbandonedOrderSweep>();

        return services;
    }
}

using ECommerce.ProductCatalog.Application.Abstractions;
using ECommerce.ProductCatalog.Application.Services;
using ECommerce.ProductCatalog.Infrastructure.Persistence;
using ECommerce.ProductCatalog.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.ProductCatalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddProductCatalogInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ProductCatalogDb")
            ?? throw new InvalidOperationException("Connection string 'ProductCatalogDb' is not configured.");

        services.AddDbContext<ProductCatalogDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ProductService>();
        services.AddScoped<CategoryService>();

        return services;
    }
}

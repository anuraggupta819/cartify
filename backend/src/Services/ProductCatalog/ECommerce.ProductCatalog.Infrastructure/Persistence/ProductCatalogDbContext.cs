using ECommerce.ProductCatalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.ProductCatalog.Infrastructure.Persistence;

public class ProductCatalogDbContext(DbContextOptions<ProductCatalogDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductCatalogDbContext).Assembly);
    }
}

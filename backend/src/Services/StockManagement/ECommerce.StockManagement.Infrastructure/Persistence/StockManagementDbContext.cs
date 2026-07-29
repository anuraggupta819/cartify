using ECommerce.StockManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.StockManagement.Infrastructure.Persistence;

public class StockManagementDbContext(DbContextOptions<StockManagementDbContext> options) : DbContext(options)
{
    public DbSet<Stock> Stocks => Set<Stock>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StockManagementDbContext).Assembly);
    }
}

using ECommerce.PaymentProcessing.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.PaymentProcessing.Infrastructure.Persistence;

public class PaymentProcessingDbContext(DbContextOptions<PaymentProcessingDbContext> options) : DbContext(options)
{
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentProcessingDbContext).Assembly);
    }
}

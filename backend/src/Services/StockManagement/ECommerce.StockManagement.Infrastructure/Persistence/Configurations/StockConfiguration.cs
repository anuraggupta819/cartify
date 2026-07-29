using ECommerce.StockManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.StockManagement.Infrastructure.Persistence.Configurations;

public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.ToTable("Stocks");
        builder.HasKey(s => s.ProductId);

        builder.Property(s => s.Quantity).IsRequired();
        builder.Property(s => s.Reserved).IsRequired();

        builder.Ignore(s => s.Available);
    }
}

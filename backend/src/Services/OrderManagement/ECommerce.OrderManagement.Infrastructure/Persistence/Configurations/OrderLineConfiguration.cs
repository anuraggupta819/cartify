using ECommerce.OrderManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.OrderManagement.Infrastructure.Persistence.Configurations;

public class OrderLineConfiguration : IEntityTypeConfiguration<OrderLine>
{
    public void Configure(EntityTypeBuilder<OrderLine> builder)
    {
        builder.ToTable("OrderLines");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.ProductName).IsRequired().HasMaxLength(200);
        builder.Property(l => l.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Ignore(l => l.LineTotal);
    }
}

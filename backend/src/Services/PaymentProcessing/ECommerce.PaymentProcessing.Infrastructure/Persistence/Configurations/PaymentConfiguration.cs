using ECommerce.PaymentProcessing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.PaymentProcessing.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.RazorpayOrderId).IsRequired().HasMaxLength(100);
        builder.Property(p => p.RazorpayPaymentId).HasMaxLength(100);
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(p => p.Amount).HasColumnType("decimal(18,2)");

        builder.HasIndex(p => p.RazorpayOrderId).IsUnique();
    }
}

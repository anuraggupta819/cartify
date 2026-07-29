using ECommerce.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Identity.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email).IsRequired().HasMaxLength(320);
        builder.Property(u => u.GoogleSub).HasMaxLength(255);
        builder.Property(u => u.Username).HasMaxLength(100);
        builder.Property(u => u.PasswordHash).HasMaxLength(500);
        builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(u => u.GoogleSub).IsUnique().HasFilter("\"GoogleSub\" IS NOT NULL");
        builder.HasIndex(u => u.Username).IsUnique().HasFilter("\"Username\" IS NOT NULL");
    }
}

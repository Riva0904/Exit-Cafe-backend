using ExitCafe.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExitCafe.Infrastructure.Persistence.Configurations;

public class CustomCakeOrderConfiguration : IEntityTypeConfiguration<CustomCakeOrder>
{
    public void Configure(EntityTypeBuilder<CustomCakeOrder> builder)
    {
        builder.ToTable("CustomCakeOrders");
        builder.Property(o => o.CustomerName).IsRequired().HasMaxLength(150);
        builder.Property(o => o.Email).IsRequired().HasMaxLength(256);
        builder.Property(o => o.Phone).IsRequired().HasMaxLength(20);
        builder.Property(o => o.Occasion).IsRequired().HasMaxLength(100);
        builder.Property(o => o.Size).IsRequired().HasMaxLength(50);
        builder.Property(o => o.Flavor).IsRequired().HasMaxLength(100);
        builder.Property(o => o.Budget).HasPrecision(10, 2);
        builder.HasIndex(o => o.Status);
    }
}

using ExitCafe.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExitCafe.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(50);
        builder.Property(o => o.SubTotal).HasPrecision(10, 2);
        builder.Property(o => o.DiscountAmount).HasPrecision(10, 2);
        builder.Property(o => o.TaxAmount).HasPrecision(10, 2);
        builder.Property(o => o.DeliveryFee).HasPrecision(10, 2);
        builder.Property(o => o.TotalAmount).HasPrecision(10, 2);
        builder.Property(o => o.DeliveryAddressLine1).HasMaxLength(250);
        builder.Property(o => o.DeliveryCity).HasMaxLength(100);
        builder.Property(o => o.DeliveryState).HasMaxLength(100);
        builder.Property(o => o.DeliveryPostalCode).HasMaxLength(20);

        builder.HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.DeliveryAddress)
            .WithMany()
            .HasForeignKey(o => o.DeliveryAddressId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");
        builder.Property(i => i.ProductName).IsRequired().HasMaxLength(200);
        builder.Property(i => i.UnitPrice).HasPrecision(10, 2);
        builder.Property(i => i.TotalPrice).HasPrecision(10, 2);

        builder.HasOne(i => i.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

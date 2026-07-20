using ExitCafe.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExitCafe.Infrastructure.Persistence.Configurations;

public class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
{
    public void Configure(EntityTypeBuilder<ContactMessage> builder)
    {
        builder.ToTable("ContactMessages");
        builder.Property(m => m.Name).IsRequired().HasMaxLength(150);
        builder.Property(m => m.Email).IsRequired().HasMaxLength(256);
        builder.Property(m => m.Subject).IsRequired().HasMaxLength(200);
        builder.Property(m => m.Message).IsRequired().HasMaxLength(4000);
    }
}

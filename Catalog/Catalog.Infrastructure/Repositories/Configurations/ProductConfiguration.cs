using Catalog.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Repositories.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnType("INT").UseIdentityColumn();
        builder.Property(p => p.Name).HasColumnType("NVARCHAR(50)").IsRequired();
        builder.Property(p => p.Description).HasColumnType("NVARCHAR(255)");
        builder.Property(p => p.Price).HasColumnType("DECIMAL(18, 2)").IsRequired();
        builder.Property(p => p.Category).HasColumnType("NVARCHAR(50)").IsRequired();
        builder.Property(p => p.Stock).HasColumnType("INT").IsRequired();
        builder.HasMany(p => p.Images)
            .WithOne(i => i.Product)
            .HasForeignKey(i => i.ProductId)
            .IsRequired();
    }
}
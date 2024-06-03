using Catalog.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Repositories.Configurations;

public class ImageConfiguration : IEntityTypeConfiguration<ImageData>
{
    public void Configure(EntityTypeBuilder<ImageData> builder)
    {
        builder.ToTable("Images");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnType("INT").UseIdentityColumn();
        builder.Property(i => i.ImageBytes).HasColumnType("VARBINARY(MAX)").IsRequired();
        builder.HasOne(i => i.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(i => i.ProductId)
            .IsRequired();
    }
}
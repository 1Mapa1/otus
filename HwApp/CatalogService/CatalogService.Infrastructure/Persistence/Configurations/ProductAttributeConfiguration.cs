using CatalogService.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Persistence.Configurations
{
    internal sealed class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductAttribute> builder)
        {
            builder.ToTable("product_attributes");

            builder.HasKey(attribute => attribute.Id);

            builder.Property(attribute => attribute.Id)
                .HasColumnName("id")
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(attribute => attribute.ProductId)
                .HasColumnName("product_id")
                .IsRequired();

            builder.Property(attribute => attribute.Name)
                .HasColumnName("name")
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(attribute => attribute.Value)
                .HasColumnName("value")
                .HasMaxLength(512)
                .IsRequired();

            builder.Property(attribute => attribute.SortOrder)
                .HasColumnName("sort_order")
                .IsRequired();

            builder.HasIndex(attribute => attribute.ProductId);
            builder.HasIndex(attribute => new { attribute.ProductId, attribute.SortOrder });
        }
    }
}

using CatalogService.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Persistence.Configurations
{
    internal sealed class ProductReadModelConfiguration : IEntityTypeConfiguration<ProductReadModel>
    {
        public void Configure(EntityTypeBuilder<ProductReadModel> builder)
        {
            builder.ToTable("product_read_models");

            builder.HasKey(model => model.ProductId);

            builder.Property(model => model.ProductId)
                .HasColumnName("product_id")
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(model => model.Name)
                .HasColumnName("name")
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(model => model.Price)
                .HasColumnName("price")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(model => model.ImageUrl)
                .HasColumnName("image_url")
                .HasMaxLength(2048)
                .IsRequired();

            builder.Property(model => model.BrandId)
                .HasColumnName("brand_id")
                .IsRequired();

            builder.Property(model => model.BrandName)
                .HasColumnName("brand_name")
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(model => model.CategoryId)
                .HasColumnName("category_id")
                .IsRequired();

            builder.Property(model => model.AttributeValuesJson)
                .HasColumnName("attribute_values_json")
                .HasColumnType("jsonb")
                .IsRequired();

            builder.Property(model => model.AvailabilityStatus)
                .HasColumnName("availability_status")
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(model => model.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            builder.Property(model => model.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired();

            builder.HasIndex(model => new { model.IsActive, model.CategoryId, model.Price });
            builder.HasIndex(model => new { model.IsActive, model.BrandId, model.Price });
            builder.HasIndex(model => model.AvailabilityStatus);
        }
    }
}

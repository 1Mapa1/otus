using CatalogService.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Persistence.Configurations
{
    internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("products");

            builder.HasKey(product => product.Id);

            builder.Property(product => product.Id)
                .HasColumnName("id")
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(product => product.Name)
                .HasColumnName("name")
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(product => product.Description)
                .HasColumnName("description")
                .HasMaxLength(4000)
                .IsRequired();

            builder.Property(product => product.BrandId)
                .HasColumnName("brand_id")
                .IsRequired();

            builder.Property(product => product.CategoryId)
                .HasColumnName("category_id")
                .IsRequired();

            builder.Property(product => product.Price)
                .HasColumnName("price")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(product => product.ImageUrl)
                .HasColumnName("image_url")
                .HasMaxLength(2048)
                .IsRequired();

            builder.Property(product => product.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            builder.Property(product => product.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(product => product.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired();

            builder.Ignore(product => product.Attributes);
            builder.Ignore(product => product.Events);

            builder.HasMany<ProductAttribute>("_attributes")
                .WithOne()
                .HasForeignKey(attribute => attribute.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation("_attributes")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(product => product.BrandId);
            builder.HasIndex(product => product.CategoryId);
            builder.HasIndex(product => product.IsActive);
        }
    }
}

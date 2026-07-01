using CatalogService.Domain.Brands;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Persistence.Configurations
{
    internal sealed class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.ToTable("brands");

            builder.HasKey(brand => brand.Id);

            builder.Property(brand => brand.Id)
                .HasColumnName("id")
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(brand => brand.Name)
                .HasColumnName("name")
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(brand => brand.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(brand => brand.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired();

            builder.HasIndex(brand => brand.Name)
                .IsUnique();
        }
    }
}

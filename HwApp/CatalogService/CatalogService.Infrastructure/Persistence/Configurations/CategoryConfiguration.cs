using CatalogService.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Persistence.Configurations
{
    internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("categories");

            builder.HasKey(category => category.Id);

            builder.Property(category => category.Id)
                .HasColumnName("id")
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(category => category.Name)
                .HasColumnName("name")
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(category => category.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(category => category.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired();

            builder.HasIndex(category => category.Name)
                .IsUnique();
        }
    }
}

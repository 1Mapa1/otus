using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseService.Domain.Products;

namespace WarehouseService.Infrastructure.Persistence.Configurations
{
    internal class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("products");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
               .HasColumnName("id")
               .ValueGeneratedNever()
               .IsRequired();

            builder.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.UnitPrice)
                .HasColumnName("unit_price")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.AvailableQuantity)
                .HasColumnName("available_quantity")
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(x => x.ReservedQuantity)
                .HasColumnName("reserved_quantity")
                .HasDefaultValue(0)
                .IsRequired();

            builder.Ignore(x => x.FreeQuantity);

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();
        }
    }
}

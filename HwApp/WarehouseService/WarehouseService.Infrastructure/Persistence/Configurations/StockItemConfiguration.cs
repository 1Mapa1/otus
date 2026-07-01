using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseService.Domain.Stocks;

namespace WarehouseService.Infrastructure.Persistence.Configurations
{
    internal sealed class StockItemConfiguration : IEntityTypeConfiguration<StockItem>
    {
        public void Configure(EntityTypeBuilder<StockItem> builder)
        {
            builder.ToTable("stock_items", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_stock_items_available_quantity",
                    "available_quantity >= 0");

                tableBuilder.HasCheckConstraint(
                    "CK_stock_items_reserved_quantity",
                    "reserved_quantity >= 0");
            });

            builder.HasKey(x => x.ProductId);

            builder.Property(x => x.ProductId)
                .HasColumnName("product_id")
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.AvailableQuantity)
                .HasColumnName("available_quantity")
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(x => x.ReservedQuantity)
                .HasColumnName("reserved_quantity")
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.Ignore(x => x.Events);
        }
    }
}

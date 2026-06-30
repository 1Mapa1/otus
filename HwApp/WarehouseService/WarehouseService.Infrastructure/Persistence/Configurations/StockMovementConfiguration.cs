using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseService.Domain.Stocks;

namespace WarehouseService.Infrastructure.Persistence.Configurations
{
    internal sealed class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
    {
        public void Configure(EntityTypeBuilder<StockMovement> builder)
        {
            builder.ToTable("stock_movements");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.ProductId)
                .HasColumnName("product_id")
                .IsRequired();

            builder.Property(x => x.Type)
                .HasColumnName("type")
                .HasConversion<string>()
                .IsRequired();

            builder.Property(x => x.Quantity)
                .HasColumnName("quantity")
                .IsRequired();

            builder.Property(x => x.OrderId)
                .HasColumnName("order_id");

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.HasIndex(x => x.ProductId);
            builder.HasIndex(x => new { x.ProductId, x.CreatedAt });

            builder.HasOne<StockItem>()
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

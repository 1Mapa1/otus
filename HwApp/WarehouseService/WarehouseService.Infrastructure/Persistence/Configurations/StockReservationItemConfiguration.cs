using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseService.Domain.Products;
using WarehouseService.Domain.StockReservations;

namespace WarehouseService.Infrastructure.Persistence.Configurations
{
    internal class StockReservationItemConfiguration : IEntityTypeConfiguration<StockReservationItem>
    {
        public void Configure(EntityTypeBuilder<StockReservationItem> builder)
        {
            builder.ToTable("stock_reservation_items");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
               .HasColumnName("id")
               .ValueGeneratedNever()
               .IsRequired();

            builder.Property(x => x.ReservationId)
               .HasColumnName("reservation_id")
               .IsRequired();

            builder.Property(x => x.ProductId)
                .HasColumnName("product_id")
                .IsRequired();

            builder.Property(x => x.Quantity)
                .HasColumnName("quantity")
                .IsRequired();

            builder.HasIndex(x => x.ReservationId);
            builder.HasIndex(x => x.ProductId);

            builder.HasOne<StockReservation>()
                .WithMany()
                .HasForeignKey(x => x.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Product>()
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

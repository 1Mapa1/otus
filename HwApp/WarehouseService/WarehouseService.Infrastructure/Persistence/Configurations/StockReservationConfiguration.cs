using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseService.Domain.StockReservations;

namespace WarehouseService.Infrastructure.Persistence.Configurations
{
    internal class StockReservationConfiguration : IEntityTypeConfiguration<StockReservation>
    {
        public void Configure(EntityTypeBuilder<StockReservation> builder)
        {
            builder.ToTable("stock_reservations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
               .HasColumnName("id")
               .ValueGeneratedNever()
               .IsRequired();

            builder.Property(x => x.OrderId)
               .HasColumnName("order_id")
               .IsRequired();

            builder.Property(x => x.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(x => x.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .IsRequired();

            builder.Property(a => a.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.Property(a => a.CanceledAt)
                .HasColumnName("canceled_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.OrderId).IsUnique();
        }
    }
}

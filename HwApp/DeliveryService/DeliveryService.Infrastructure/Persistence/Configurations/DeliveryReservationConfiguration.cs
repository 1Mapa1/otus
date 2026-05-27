using DeliveryService.Domain.Reservations;
using DeliveryService.Domain.Slots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryService.Infrastructure.Persistence.Configurations
{
    internal class DeliveryReservationConfiguration : IEntityTypeConfiguration<DeliveryReservation>
    {
        public void Configure(EntityTypeBuilder<DeliveryReservation> builder)
        {
            builder.ToTable("delivery_reservation");

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

            builder.Property(x => x.DeliverySlotId)
                .HasColumnName("delivery_slot_id")
                .IsRequired();

            builder.Property(x => x.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.Property(x => x.CanceledAt)
                .HasColumnName("canceled_at");

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.OrderId).IsUnique();
            builder.HasIndex(x => x.DeliverySlotId);

            builder.HasOne<DeliverySlot>()
                .WithMany()
                .HasForeignKey(x => x.DeliverySlotId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

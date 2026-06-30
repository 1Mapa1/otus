using DeliveryService.Domain.Reservations;
using DeliveryService.Domain.Slots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryService.Infrastructure.Persistence.Configurations
{
    internal sealed class DeliveryReservationConfiguration : IEntityTypeConfiguration<DeliveryReservation>
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

            builder.Property(x => x.CustomerId)
                .HasColumnName("customer_id")
                .IsRequired();

            builder.Property(x => x.DeliverySlotId)
                .HasColumnName("delivery_slot_id")
                .IsRequired();

            builder.Property(x => x.ZoneId)
                .HasColumnName("zone_id")
                .IsRequired();

            builder.Property(x => x.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .IsRequired();

            builder.OwnsOne(x => x.DeliveryAddress, address =>
            {
                address.Property(a => a.City)
                    .HasColumnName("address_city")
                    .HasMaxLength(128)
                    .IsRequired();

                address.Property(a => a.Street)
                    .HasColumnName("address_street")
                    .HasMaxLength(256)
                    .IsRequired();

                address.Property(a => a.House)
                    .HasColumnName("address_house")
                    .HasMaxLength(32)
                    .IsRequired();

                address.Property(a => a.Apartment)
                    .HasColumnName("address_apartment")
                    .HasMaxLength(32);
            });

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.Property(x => x.CanceledAt)
                .HasColumnName("canceled_at");

            builder.HasIndex(x => x.CustomerId);
            builder.HasIndex(x => x.OrderId).IsUnique();
            builder.HasIndex(x => x.DeliverySlotId);
            builder.HasIndex(x => x.ZoneId);

            builder.HasOne<DeliverySlot>()
                .WithMany()
                .HasForeignKey(x => x.DeliverySlotId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

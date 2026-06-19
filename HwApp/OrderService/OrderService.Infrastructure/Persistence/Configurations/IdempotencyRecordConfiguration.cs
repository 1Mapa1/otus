using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.IdempotencyRecords;
using OrderService.Domain.Orders;

namespace OrderService.Infrastructure.Persistence.Configurations
{
    internal sealed class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
    {
        public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
        {
            builder.ToTable("idempotency_records");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(x => x.IdempotencyKey)
                .HasColumnName("idempotency_key")
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(x => x.RequestHash)
                .HasColumnName("request_hash")
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(x => x.OrderId)
                .HasColumnName("order_id");

            builder.Property(x => x.ResponseBody)
                .HasColumnName("response_body");

            builder.Property(x => x.LockedUntil)
               .HasColumnName("locked_until");

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.Property(x => x.ExpiresAt)
               .HasColumnName("expires_at")
               .IsRequired();

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.ExpiresAt);

            builder.HasIndex(x => new { x.UserId, x.IdempotencyKey })
                .IsUnique();

            builder.HasOne<Order>()
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

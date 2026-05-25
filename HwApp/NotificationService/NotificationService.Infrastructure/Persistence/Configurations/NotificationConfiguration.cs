using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationService.Domain.Notifications;

namespace NotificationService.Infrastructure.Persistence.Configurations
{
    internal sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("notifications");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(x => x.OrderId)
                .HasColumnName("order_id");

            builder.Property(x => x.Type)
                .HasColumnName("type")
                .HasConversion<string>()
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(x => x.Subject)
                .HasColumnName("subject")
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(x => x.Body)
                .HasColumnName("body")
                .HasMaxLength(4000)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.OrderId);
        }
    }
}

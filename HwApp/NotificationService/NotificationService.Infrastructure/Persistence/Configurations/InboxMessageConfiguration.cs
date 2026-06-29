using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationService.Infrastructure.Persistence.Entities;

namespace NotificationService.Infrastructure.Persistence.Configurations
{
    internal sealed class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
    {
        public void Configure(EntityTypeBuilder<InboxMessage> builder)
        {
            builder.ToTable("inbox_messages");

            builder.HasKey(x => x.EventId);

            builder.Property(x => x.EventId)
                .HasColumnName("event_id")
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.EventType)
                .HasColumnName("event_type")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.KafkaTopic)
                .HasColumnName("kafka_topic")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.KafkaPartition)
                .HasColumnName("kafka_partition")
                .IsRequired();

            builder.Property(x => x.KafkaOffset)
                .HasColumnName("kafka_offset")
                .IsRequired();

            builder.Property(x => x.OccurredAtUtc)
                .HasColumnName("occurred_at_utc")
                .IsRequired();

            builder.Property(x => x.ProcessedAtUtc)
                .HasColumnName("processed_at_utc")
                .IsRequired();
        }
    }
}

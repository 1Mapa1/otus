using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Infrastructure.Persistence.Outbox;

namespace OrderService.Infrastructure.Persistence.Configurations
{
    internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("outbox_messages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.Topic)
                .HasColumnName("topic")
                .HasMaxLength(256);

            builder.Property(x => x.Key)
                .HasColumnName("key")
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(x => x.Payload)
                .HasColumnName("payload")
                .HasColumnType("text")
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.Property(x => x.PublishedAt)
                .HasColumnName("published_at");
        }
    }
}

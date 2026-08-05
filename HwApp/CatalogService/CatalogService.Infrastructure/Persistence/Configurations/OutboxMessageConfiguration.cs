using CatalogService.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Persistence.Configurations
{
    internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("outbox_messages");

            builder.HasKey(message => message.Id);

            builder.Property(message => message.Id)
                .HasColumnName("id")
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(message => message.Topic)
                .HasColumnName("topic")
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(message => message.Key)
                .HasColumnName("key")
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(message => message.Payload)
                .HasColumnName("payload")
                .HasColumnType("text")
                .IsRequired();

            builder.Property(message => message.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.Property(message => message.PublishedAt)
                .HasColumnName("published_at");
        }
    }
}

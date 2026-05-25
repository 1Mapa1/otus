using CustomerService.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerService.Infrastructure.Persistence.Configurations
{
    internal class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("outbox_messages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            builder.Property(x => x.Topic)
                .IsRequired()
                .HasColumnName("topic")
                .HasMaxLength(50);

            builder.Property(x => x.Key)
                .IsRequired()
                .HasColumnName("key")
                .HasMaxLength(50);

            builder.Property(x => x.Payload)
                .IsRequired()
                .HasColumnName("payload")
                .HasMaxLength(1000);

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(x => x.PublishedAt)
                .HasColumnName("published_at");

        }
    }
}

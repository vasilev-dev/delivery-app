using DeliveryApp.Infrastructure.Adapters.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.Outbox;

public class OutboxConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox");

        builder.HasKey(outbox => outbox.Id);
        
        builder
            .Property(outbox => outbox.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();
        
        builder
            .Property(outbox => outbox.Type)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired();

        builder
            .Property(outbox => outbox.Content)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired();

        builder
            .Property(outbox => outbox.OccurredOnUtc)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired();

        builder
            .Property(outbox => outbox.ProcessedOnUtc)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired(false);

    }
}
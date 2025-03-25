using DeliveryApp.Core.Domain.Model.CourierAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.CourierAggregate;

public class TransportConfiguration : IEntityTypeConfiguration<Transport>
{
    public void Configure(EntityTypeBuilder<Transport> builder)
    {
        builder.HasKey(transport => transport.Id);
        
        builder
            .Property(transport => transport.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder
            .Property(transport => transport.Name)
            .IsRequired();
        
        builder
            .Property(transport => transport.Speed)
            .IsRequired();
    }
}
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.OrderAggregate;

public class OrderAggregate : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(order => order.Id);

        builder
            .Property(order => order.Id)
            .ValueGeneratedNever()
            .IsRequired();
        
        builder
            .Property(order => order.CourierId)
            .IsRequired(false);
        
        builder
            .Property(order => order.Status)
            .IsRequired();
        
        builder
            .OwnsOne(courier => courier.Location, navigation =>
            {
                navigation.Property(location => location.X).IsRequired();
                navigation.Property(location => location.Y).IsRequired();
                navigation.WithOwner();
            });
        
        builder.Navigation(courier => courier.Location).IsRequired();
    }
}
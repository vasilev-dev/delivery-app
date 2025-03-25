using DeliveryApp.Core.Domain.Model.CourierAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.CourierAggregate;

public class CourierConfiguration : IEntityTypeConfiguration<Courier>
{
    public void Configure(EntityTypeBuilder<Courier> builder)
    {
        builder.HasKey(courier => courier.Id);
        
        builder
            .Property(courier => courier.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder
            .Property(courier => courier.Name)
            .IsRequired();
        
        builder
            .Property(courier => courier.Status)
            .IsRequired();

        builder
            .OwnsOne(courier => courier.Location, navigation =>
            {
                navigation.Property(location => location.X).IsRequired();
                navigation.Property(location => location.Y).IsRequired();
                navigation.WithOwner();
            });
        
        builder.Navigation(courier => courier.Location).IsRequired();

        builder
            .HasOne(courier => courier.Transport)
            .WithMany()
            .IsRequired();
    }
}
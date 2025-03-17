using System.Reflection;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using SmartEnum.EFCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<Courier> Couriers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.ConfigureSmartEnum();
    }
}
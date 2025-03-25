using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;

public class OrderRepository(AppDbContext dbContext) : IOrderRepository
{
    public async Task<Order> GetById(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Orders.FindAsync([orderId], cancellationToken);
    }

    public async Task<List<Order>> GetAllCreated(CancellationToken cancellationToken = default)
    {
        return await dbContext.Orders
            .Where(order => order.Status == OrderStatus.Created)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Order>> GetAllAssigned(CancellationToken cancellationToken = default)
    {
        return await dbContext.Orders
            .Where(order => order.Status == OrderStatus.Assigned)
            .ToListAsync(cancellationToken);
    }

    public async Task Add(Order order, CancellationToken cancellationToken = default)
    {
        await dbContext.Orders.AddAsync(order, cancellationToken);
    }

    public void Update(Order order)
    {
        dbContext.Orders.Update(order);
    }
}
using DeliveryApp.Core.Domain.Model.OrderAggregate;

namespace DeliveryApp.Core.Ports;

public interface IOrderRepository
{
    Task<Order> GetById(Guid orderId, CancellationToken cancellationToken = default);
    Task<List<Order>> GetAllCreated(CancellationToken cancellationToken = default);
    Task<List<Order>> GetAllAssigned(CancellationToken cancellationToken = default);
    Task Add(Order order, CancellationToken cancellationToken = default);
    void Update(Order order);
}
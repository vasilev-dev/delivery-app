using DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;

namespace DeliveryApp.Core.Ports;

public interface IMessageBusProducer
{
    Task PublishOrderStatusChangedDomainEvent(OrderStatusChangedDomainEvent @event, CancellationToken cancellationToken);
}
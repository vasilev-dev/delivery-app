using DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.DomainEventHandlers;

public class OrderStatusChangedDomainEventHandler(IMessageBusProducer producer) : INotificationHandler<OrderStatusChangedDomainEvent>
{
    public async Task Handle(OrderStatusChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        await producer.PublishOrderStatusChangedDomainEvent(notification, cancellationToken);
    }
}
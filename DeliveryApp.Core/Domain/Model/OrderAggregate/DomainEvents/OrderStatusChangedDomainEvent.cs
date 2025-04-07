using Primitives;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;

public record OrderStatusChangedDomainEvent(Guid OrderId, OrderStatus Status) : DomainEvent;
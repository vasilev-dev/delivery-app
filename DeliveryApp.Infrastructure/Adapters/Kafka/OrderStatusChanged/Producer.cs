using System.Text.Json;
using Confluent.Kafka;
using DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Ports;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderStatusChanged;
using DomainOrderStatus = DeliveryApp.Core.Domain.Model.OrderAggregate.OrderStatus;

namespace DeliveryApp.Infrastructure.Adapters.Kafka.OrderStatusChanged;

public class Producer(IOptions<Settings> settings, ILogger<Producer> logger) : IMessageBusProducer
{
    private readonly string _topic = settings.Value.OrderStatusChangedTopic;
    private readonly ProducerConfig _config = new()
    {
        BootstrapServers = settings.Value.MessageBrokerHost
    };

    public async Task PublishOrderStatusChangedDomainEvent(OrderStatusChangedDomainEvent @event, CancellationToken cancellationToken)
    {
        try
        {
            var integrationEvent = new OrderStatusChangedIntegrationEvent
            {
                OrderId = @event.OrderId.ToString(),
                OrderStatus = MapStatus(@event.Status)
            };
            
            var message = new Message<string, string>
            {
                Key = @event.EventId.ToString(),
                Value = JsonSerializer.Serialize(integrationEvent)
            };

            using var producer = new ProducerBuilder<string, string>(_config).Build();
            await producer.ProduceAsync(_topic, message, cancellationToken);
        }
        catch (ProduceException<Null, string> e)
        {
            logger.LogError("Delivery failed: {reason}", e.Error.Reason);
            throw;
        }
    }

    private static OrderStatus MapStatus(DomainOrderStatus status)
    {
        if (status.Value == DomainOrderStatus.Created) return OrderStatus.Created;
        if (status.Value == DomainOrderStatus.Assigned) return OrderStatus.Assigned;
        if (status.Value == DomainOrderStatus.Completed) return OrderStatus.Completed;

        return OrderStatus.None;
    }
}
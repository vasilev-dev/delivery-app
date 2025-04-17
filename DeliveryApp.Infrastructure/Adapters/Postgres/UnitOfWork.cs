using Ardalis.SmartEnum.JsonNet;
using DeliveryApp.Infrastructure.Adapters.Postgres.Entities;
using Newtonsoft.Json;
using Primitives;
using DomainOrderStatus = DeliveryApp.Core.Domain.Model.OrderAggregate.OrderStatus;

namespace DeliveryApp.Infrastructure.Adapters.Postgres;

public class UnitOfWork(AppDbContext dbContext) : IUnitOfWork, IDisposable
{
    private bool _disposed;
    
    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await SaveDomainEventsInOutboxMessagesAsync(cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return true;
    }
    
    private async Task SaveDomainEventsInOutboxMessagesAsync(CancellationToken cancellationToken)
    {
        var outboxMessages = dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .Select(x => x.Entity)
            .SelectMany(aggregate =>
                {
                    var domainEvents = aggregate.GetDomainEvents();
                    aggregate.ClearDomainEvents();
                    return domainEvents;
                }
            )
            .Select(domainEvent => new OutboxMessage
            {
                Id = domainEvent.EventId,
                OccurredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(
                    domainEvent,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        Converters = new List<JsonConverter>
                        {
                            new SmartEnumNameConverter<DomainOrderStatus, int>()
                        }
                    })

            })
            .ToList();
        
        await dbContext.Set<OutboxMessage>().AddRangeAsync(outboxMessages, cancellationToken);
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        
        if (disposing) dbContext.Dispose();
        _disposed = true;
    }
}
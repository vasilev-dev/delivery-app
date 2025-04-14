using System.Collections.Immutable;
using MediatR;
using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Postgres;

public class UnitOfWork(AppDbContext dbContext, IMediator mediator) : IUnitOfWork, IDisposable
{
    private bool _disposed;
    
    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
        await RaiseDomainEventAsync(cancellationToken);
        
        return true;
    }

    private async Task RaiseDomainEventAsync(CancellationToken cancellationToken)
    {
        var domainEntities = dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(x => x.Entity.GetDomainEvents().Count != 0)
            .ToImmutableArray();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.GetDomainEvents())
            .ToImmutableArray();

        domainEntities
            .ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent, cancellationToken);
        }
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
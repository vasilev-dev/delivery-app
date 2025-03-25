using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Postgres;

public class UnitOfWork(AppDbContext dbContext) : IUnitOfWork, IDisposable
{
    private bool _disposed;
    
    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return true;
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
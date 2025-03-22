using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;

public class CourierRepository(AppDbContext dbContext) : ICourierRepository
{
    public async Task<Courier> GetById(Guid courierId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Couriers.FindAsync([courierId], cancellationToken);
    }

    public async Task<List<Courier>> GetAllFree(CancellationToken cancellationToken = default)
    {
        return await dbContext.Couriers
            .Include(courier => courier.Transport)
            .Where(courier => courier.Status == CourierStatus.Free)
            .ToListAsync(cancellationToken);
    }

    public async Task Add(Courier courier, CancellationToken cancellationToken = default)
    {
        await dbContext.Couriers.AddAsync(courier, cancellationToken);
    }

    public void Update(Courier courier)
    {
        dbContext.Couriers.Update(courier);
    }
}
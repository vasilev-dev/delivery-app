using DeliveryApp.Core.Domain.Model.CourierAggregate;
using Primitives;

namespace DeliveryApp.Core.Ports;

public interface ICourierRepository : IRepository<Courier>
{
    Task<Courier> GetById(Guid courierId, CancellationToken cancellationToken = default);
    Task<List<Courier>> GetAllFree(CancellationToken cancellationToken = default);
    Task<List<Courier>> GetAllBusy(CancellationToken cancellationToken = default);
    Task Add(Courier courier, CancellationToken cancellationToken = default);
    void Update(Courier courier);
}
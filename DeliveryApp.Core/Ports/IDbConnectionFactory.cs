using System.Data;

namespace DeliveryApp.Core.Ports;

public interface IDbConnectionFactory
{
    Task<IDbConnection> GetConnectionAsync(CancellationToken cancellationToken = default);
}
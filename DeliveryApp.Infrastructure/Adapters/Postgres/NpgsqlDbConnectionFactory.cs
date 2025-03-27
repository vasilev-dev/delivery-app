using System.Data;
using DeliveryApp.Core.Ports;
using Npgsql;

namespace DeliveryApp.Infrastructure.Adapters.Postgres;

public class NpgsqlDbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public NpgsqlDbConnectionFactory(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        _connectionString = connectionString;
    }
    
    public async Task<IDbConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        
        return connection;
    }
}
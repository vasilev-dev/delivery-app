using Dapper;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetCouriers;

public sealed class GetCouriersHandler(IDbConnectionFactory dbConnectionFactory)
    : IRequestHandler<GetCouriersQuery, GetCouriersResult>
{
    public async Task<GetCouriersResult> Handle(GetCouriersQuery request, CancellationToken cancellationToken)
    {
        var connection = await dbConnectionFactory.GetConnectionAsync(cancellationToken);

        var result = await connection.QueryAsync(
            """
            select
                id,
                name,
                location_x,
                location_y,
                transport_id
            from
                couriers
            """
        );
        
        connection.Close();

        var couriers = result.Select(Map);
        
        return new GetCouriersResult(couriers);
    }

    private static Courier Map(dynamic result)
    {
        var location = new Location(result.location_x, result.location_y);
        
        return new Courier(result.id, result.name, location, result.transport_id);
    }
}
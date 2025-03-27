using Dapper;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers;

public sealed class GetBusyCouriersHandler(IDbConnectionFactory dbConnectionFactory)
    : IRequestHandler<GetBusyCouriersQuery, GetBusyCouriersResult>
{
    public async Task<GetBusyCouriersResult> Handle(GetBusyCouriersQuery request, CancellationToken cancellationToken)
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
            where
                status = @status
            """,
            new { status = CourierStatus.Busy.Value }
        );

        var couriers = result.Select(Map);
        
        return new GetBusyCouriersResult(couriers);
    }

    private static Courier Map(dynamic result)
    {
        var location = new Location(result.location_x, result.location_y);
        
        return new Courier(result.id, result.name, location, result.transport_id);
    }
}
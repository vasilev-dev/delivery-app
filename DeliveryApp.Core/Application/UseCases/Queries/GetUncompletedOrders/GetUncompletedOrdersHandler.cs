using Dapper;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetUncompletedOrders;

public sealed class GetUncompletedOrdersHandler(IDbConnectionFactory dbConnectionFactory)
    : IRequestHandler<GetUncompletedOrdersQuery, GetUncompletedOrdersResult>
{
    public async Task<GetUncompletedOrdersResult> Handle(GetUncompletedOrdersQuery request, CancellationToken cancellationToken)
    {
        var connection = await dbConnectionFactory.GetConnectionAsync(cancellationToken);

        var result = await connection.QueryAsync(
            """
            select
                id,
                location_x,
                location_y
            from
                orders
            where
                status != @status
            """, 
            new { status = OrderStatus.Completed.Value }
        );

        var orders = result.Select(Map);
        
        return new GetUncompletedOrdersResult(orders);
    }

    private static Order Map(dynamic result)
    {
        var location = new Location(result.location_x, result.location_y);

        return new Order(result.id, location);
    }
}
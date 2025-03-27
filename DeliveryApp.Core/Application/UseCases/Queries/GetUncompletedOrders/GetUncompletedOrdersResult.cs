namespace DeliveryApp.Core.Application.UseCases.Queries.GetUncompletedOrders;

public record GetUncompletedOrdersResult(IEnumerable<Order> orders);

public record Order(Guid Id, Location Location);

public record Location(int X, int Y);
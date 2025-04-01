namespace DeliveryApp.Core.Application.UseCases.Queries.GetCouriers;

public record GetCouriersResult(IEnumerable<Courier> Couriers);

public record Courier(Guid Id, string Name, Location Location, Guid TransportId);

public record Location(int X, int Y);
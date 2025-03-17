using System.Collections.Immutable;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Domain.Services;

public class DispatchService : IDispatchService
{
    public Result<Courier, Error> AssignSuitableCourier(Order order, IReadOnlyCollection<Courier> couriers)
    {
        if (order is null) return GeneralErrors.ValueIsRequired(nameof(order));
        if (couriers is null) return GeneralErrors.ValueIsRequired(nameof(couriers));
        
        var freeCouriers = couriers
            .Where(courier => courier.Status == CourierStatus.Free)
            .ToImmutableArray();

        if (freeCouriers.Length == 0) return Errors.NoFreeCouriers;

        Courier fastestCourier = null;
        var minTimeToLocation = int.MaxValue;

        foreach (var courier in freeCouriers)
        {
            var timeToLocationResult = courier.CalculateTimeToLocation(order.Location);

            if (timeToLocationResult.IsFailure) return timeToLocationResult.Error;

            if (timeToLocationResult.Value >= minTimeToLocation) continue;
            
            minTimeToLocation = timeToLocationResult.Value;
            fastestCourier = courier;
        }

        var assignResult = order.Assign(fastestCourier);
        if (assignResult.IsFailure) return assignResult.Error;

        var setBusyResult = fastestCourier!.SetBusy();
        if (setBusyResult.IsFailure) return setBusyResult.Error;
        
        return fastestCourier;
    }
}

public static class Errors
{
    public static Error NoFreeCouriers => new(
        $"{nameof(DispatchService).ToLowerInvariant()}.can.not.assign.courier",
        "Нет свободных курьеров"
    );
}
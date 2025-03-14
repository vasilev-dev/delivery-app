using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate;

public class Order : Aggregate<Guid>
{
    public Guid? CourierId { get; private set; }
    public OrderStatus Status { get; private set; }
    public Location Location { get; private set; }

    private Order(Guid orderId, Location location) : base(orderId)
    {
        Location = location;
        Status = OrderStatus.Created;
    }

    public static Result<Order, Error> Create(Guid orderId, Location location)
    {
        if (orderId == Guid.Empty) return GeneralErrors.ValueIsRequired(nameof(orderId));
        if (location is null) return GeneralErrors.ValueIsRequired(nameof(location));
        
        return new Order(orderId, location);
    }

    public UnitResult<Error> Assign(Courier courier)
    {
        if (courier is null) return GeneralErrors.ValueIsRequired(nameof(courier));
        if (Status != OrderStatus.Created) return Errors.CannotAssignCourierForNotCreatedOrderStatus;
        
        CourierId = courier.Id;
        Status = OrderStatus.Assigned;
        
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> Complete()
    {
        if (Status != OrderStatus.Assigned) return Errors.CannotCompleteNotAssignedOrder;
        
        Status = OrderStatus.Completed;
        
        return UnitResult.Success<Error>();
    }
}

public static class Errors
{
    public static Error CannotAssignCourierForNotCreatedOrderStatus => new(
        $"{nameof(Order).ToLowerInvariant()}.cannot.assign.courier.for.not.created.order.status",
        $"Нельзя назначить курьера на заказ со статусом не {OrderStatus.Created.Name}"
    );
    
    public static Error CannotCompleteNotAssignedOrder => new(
        $"{nameof(Order).ToLowerInvariant()}.cannot.complete.not.assigned.order",
        "Нельзя завершить не назначенный на курьера заказ"
    );
}
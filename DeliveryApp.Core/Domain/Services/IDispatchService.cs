using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Domain.Services;

public interface IDispatchService
{
    Result<Courier, Error> AssignSuitableCourier(Order order, IReadOnlyCollection<Courier> couriers);
}
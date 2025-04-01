using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;

public sealed class CreateOrderHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IGeoClient geoClient
) : IRequestHandler<CreateOrderCommand, UnitResult<Error>>
{
    public async Task<UnitResult<Error>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var existedOrder = await orderRepository.GetById(request.BasketId, cancellationToken);

        if (existedOrder is not null) return UnitResult.Failure(Errors.OrderAlreadyExists(request.BasketId));
        
        var getLocationResult = await geoClient.GetGeolocation(request.Street, cancellationToken);
        if (getLocationResult.IsFailure) return UnitResult.Failure(getLocationResult.Error);
        
        var createOrderResult = Order.Create(request.BasketId, Location.CreateRandom());
        if (createOrderResult.IsFailure) return createOrderResult;
        
        var order = createOrderResult.Value;

        await orderRepository.Add(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return UnitResult.Success<Error>();
    }
}

public static class Errors
{
    public static Error OrderAlreadyExists(Guid orderId) => new(
        $"{nameof(CreateOrderCommand).ToLowerInvariant()}.order.already.exists",
        $"Заказ {orderId} уже существует"
    );
}
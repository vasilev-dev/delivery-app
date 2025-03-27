using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.AssignCourier;

public sealed class AssignCourierHandler(
    IOrderRepository orderRepository,
    ICourierRepository courierRepository,
    IDispatchService dispatchService,
    IUnitOfWork unitOfWork
) : IRequestHandler<AssignCourierCommand, UnitResult<Error>>
{
    public async Task<UnitResult<Error>> Handle(AssignCourierCommand request, CancellationToken cancellationToken)
    {
        var orders = await orderRepository.GetAllCreated(cancellationToken);

        if (orders.Count == 0) return UnitResult.Success<Error>();
        
        var order = orders.First();
        
        var freeCouriers = await courierRepository.GetAllFree(cancellationToken);
        
        var dispatchResult = dispatchService.AssignSuitableCourier(order, freeCouriers);
        if (dispatchResult.IsFailure) return dispatchResult;
        var courier = dispatchResult.Value;

        courier.SetBusy();
        order.Assign(courier);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return UnitResult.Success<Error>();
    }
}
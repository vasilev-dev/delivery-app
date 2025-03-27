using CSharpFunctionalExtensions;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.MoveCouriers;

public sealed class MoveCouriersHandler(
    ICourierRepository courierRepository,
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<MoveCouriersCommand, UnitResult<Error>>
{
    public async Task<UnitResult<Error>> Handle(MoveCouriersCommand request, CancellationToken cancellationToken)
    {
        var busyCouriers = await courierRepository.GetAllBusy(cancellationToken);
        var assignedOrders = await orderRepository.GetAllAssigned(cancellationToken);

        foreach (var courier in busyCouriers)
        {
            var courierOrder = assignedOrders.FirstOrDefault(order => order.CourierId == courier.Id);

            if (courierOrder is null) continue;

            var moveResult = courier.Move(courierOrder.Location);

            if (moveResult.IsFailure) return moveResult;
                
            if (courierOrder.Location != courier.Location) continue;

            courier.SetFree();
            courierOrder.Complete();
        }
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return UnitResult.Success<Error>();
    }
}
using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<UnitResult<Error>>
{
    public Guid BasketId { get; }
    public string Street { get; }
    
    public CreateOrderCommand(Guid basketId, string street)
    {
        if (basketId == Guid.Empty) throw new ArgumentNullException(nameof(basketId));
        if (string.IsNullOrWhiteSpace(street)) throw new ArgumentNullException(nameof(street));
        
        BasketId = basketId;
        Street = street;
    }
}
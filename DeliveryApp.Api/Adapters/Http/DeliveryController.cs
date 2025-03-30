using DeliveryApp.Api.Adapters.Http.Contract.src.OpenApi.Controllers;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Application.UseCases.Queries.GetCouriers;
using DeliveryApp.Core.Application.UseCases.Queries.GetUncompletedOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Courier = DeliveryApp.Api.Adapters.Http.Contract.src.OpenApi.Models.Courier;
using Location = DeliveryApp.Api.Adapters.Http.Contract.src.OpenApi.Models.Location;
using Order = DeliveryApp.Api.Adapters.Http.Contract.src.OpenApi.Models.Order;

namespace DeliveryApp.Api.Adapters.Http;

public class DeliveryController(IMediator mediator) : DefaultApiController
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public override async Task<IActionResult> CreateOrder()
    {
        var orderId = Guid.NewGuid();
        const string street = "ул. Пушкина";
        var command = new CreateOrderCommand(orderId, street);
        
        var response = await _mediator.Send(command);
        
        if (response.IsSuccess) return Ok();
        
        return Conflict(response.Error.Message);
    }

    public override async Task<IActionResult> GetOrders()
    {
        var getActiveOrdersQuery = new GetUncompletedOrdersQuery();
        var response = await _mediator.Send(getActiveOrdersQuery);
        
        if (response == null) return NotFound();
        var model = response.Orders.Select(o => new Order
        {
            Id = o.Id,
            Location = new Location { X = o.Location.X, Y = o.Location.Y }
        });
        
        return Ok(model);
    }

    public override async Task<IActionResult> GetCouriers()
    {
        var getAllCouriersQuery = new GetCouriersQuery();
        var response = await _mediator.Send(getAllCouriersQuery);
        
        if (response == null) return NotFound();
        
        var model = response.Couriers.Select(c => new Courier
        {
            Id = c.Id,
            Name = c.Name,
            Location = new Location { X = c.Location.X, Y = c.Location.Y }
        });
        
        return Ok(model);
    }
}
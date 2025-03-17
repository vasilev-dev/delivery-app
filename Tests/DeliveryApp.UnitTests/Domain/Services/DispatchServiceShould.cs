using System;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Domain.Services;
using FluentAssertions;
using Xunit;
using Errors = DeliveryApp.Core.Domain.Services.Errors;

namespace DeliveryApp.UnitTests.Domain.Services;

public class DispatchServiceShould
{
    [Fact]
    public void NotAssignCourierWhenOrderIsNull()
    {
        var service = new DispatchService();
        
        var actual = service.AssignSuitableCourier(null, []);
        
        actual.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void NotAssignCourierWhenCouriersIsNull()
    {
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom()).Value;
        var service = new DispatchService();
        
        var actual = service.AssignSuitableCourier(order, null);
        
        actual.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void NotAssignCourierWhenNoFreeCouriers()
    {
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom()).Value;

        var courier = Courier.Create("Ivan", "Tesla", 3, Location.CreateRandom()).Value;
        courier.SetBusy();
        
        var service = new DispatchService();
        
        var actual = service.AssignSuitableCourier(order, [courier]);
        
        actual.IsSuccess.Should().BeFalse();
        actual.Error.Should().Be(Errors.NoFreeCouriers);
    }
    
    [Fact]
    public void AssignFastestCourier()
    {
        var orderLocation = Location.Create(1, 1).Value;
        var order = Order.Create(Guid.NewGuid(), orderLocation).Value;
        
        var couriersLocation = Location.Create(10, 10).Value;
        var slowestCourier = Courier.Create("Petr", "Lada", 2, couriersLocation).Value;
        var fastestCourier = Courier.Create("Ivan", "Tesla", 3, couriersLocation).Value;
        
        var service = new DispatchService();
        
        var actual = service.AssignSuitableCourier(order, [slowestCourier, fastestCourier]);
        
        actual.IsSuccess.Should().BeTrue();
        actual.Value.Should().Be(fastestCourier);
        
        order.CourierId.Should().Be(fastestCourier.Id);
        order.Status.Should().Be(OrderStatus.Assigned);
        
        fastestCourier.Status.Should().Be(CourierStatus.Busy);
    }
    
    [Fact]
    public void AssignNearestCourier()
    {
        var orderLocation = Location.Create(1, 1).Value;
        var order = Order.Create(Guid.NewGuid(), orderLocation).Value;
        
        var farthestCourierLocation = Location.Create(10, 10).Value;
        var farthestCourier = Courier.Create("Petr", "Tesla", 3, farthestCourierLocation).Value;
        
        var nearestCourierLocation = Location.Create(5, 5).Value;
        var nearestCourier = Courier.Create("Petr", "Tesla", 3, nearestCourierLocation).Value;
        
        var service = new DispatchService();
        
        var actual = service.AssignSuitableCourier(order, [farthestCourier, nearestCourier]);
        
        actual.IsSuccess.Should().BeTrue();
        actual.Value.Should().Be(nearestCourier);
        
        order.CourierId.Should().Be(nearestCourier.Id);
        order.Status.Should().Be(OrderStatus.Assigned);
        
        nearestCourier.Status.Should().Be(CourierStatus.Busy);
    }
}
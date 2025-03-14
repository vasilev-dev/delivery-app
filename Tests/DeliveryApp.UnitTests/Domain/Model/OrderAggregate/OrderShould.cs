using System;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using Xunit;
using Errors = DeliveryApp.Core.Domain.Model.OrderAggregate.Errors;

namespace DeliveryApp.UnitTests.Domain.Model.OrderAggregate;

public class OrderShould
{
    [Fact]
    public void NotBeCreatedWhenOrderIdIsEmpty()
    {
        var location = Location.CreateRandom();
        
        var actual = Order.Create(Guid.Empty, location);
        
        actual.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void NotBeCreatedWhenLocationIsNull()
    {
        var actual = Order.Create(Guid.NewGuid(), null);
        
        actual.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void BeCreatedWhenOrderAndLocationIsValid()
    {
        var location = Location.CreateRandom();
        var orderId = Guid.NewGuid();
        
        var actual = Order.Create(orderId, location);
        
        actual.IsSuccess.Should().BeTrue();
        actual.Value.Id.Should().Be(orderId);
        actual.Value.Location.Should().Be(location);
        actual.Value.Status.Should().Be(OrderStatus.Created);
        actual.Value.CourierId.Should().BeNull();
    }

    [Fact]
    public void NotBeAssignedWhenCourierIsNull()
    {
        var location = Location.CreateRandom();
        var orderId = Guid.NewGuid();
        var order = Order.Create(orderId, location).Value;

        var actual = order.Assign(null);
        
        actual.IsSuccess.Should().BeFalse();
    }
    
    [Fact]
    public void NotBeAssignedWhenStatusIsAssigned()
    {
        var location = Location.CreateRandom();
        var orderId = Guid.NewGuid();
        var order = Order.Create(orderId, location).Value;
        var courier = Courier.Create("Ivan", "Tesla", 3, Location.CreateRandom()).Value;

        order.Assign(courier);
        
        var actual = order.Assign(courier);
        
        actual.IsSuccess.Should().BeFalse();
        actual.Error.Should().Be(Errors.CannotAssignCourierForNotCreatedOrderStatus);
    }
    
    [Fact]
    public void NotBeAssignedWhenStatusIsCompleted()
    {
        var location = Location.CreateRandom();
        var orderId = Guid.NewGuid();
        var order = Order.Create(orderId, location).Value;
        var courier = Courier.Create("Ivan", "Tesla", 3, Location.CreateRandom()).Value;

        order.Assign(courier);
        order.Complete();
        
        var actual = order.Assign(courier);
        
        actual.IsSuccess.Should().BeFalse();
        actual.Error.Should().Be(Errors.CannotAssignCourierForNotCreatedOrderStatus);
    }
    
    [Fact]
    public void BeAssignedWhenCourierAndStatusAreValid()
    {
        var location = Location.CreateRandom();
        var orderId = Guid.NewGuid();
        var order = Order.Create(orderId, location).Value;
        var courier = Courier.Create("Ivan", "Tesla", 3, Location.CreateRandom()).Value;

        var actual = order.Assign(courier);
        
        actual.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Assigned);
        order.CourierId.Should().Be(courier.Id);
    }

    [Fact]
    public void NotBeCompletedWhenStatusIsCreated()
    {
        var location = Location.CreateRandom();
        var orderId = Guid.NewGuid();
        var order = Order.Create(orderId, location).Value;
        
        var actual = order.Complete();
        actual.IsSuccess.Should().BeFalse();
        actual.Error.Should().Be(Errors.CannotCompleteNotAssignedOrder);
    }

    [Fact]
    public void NotBeCompletedWhenStatusAlreadyIsCompleted()
    {
        var location = Location.CreateRandom();
        var orderId = Guid.NewGuid();
        var order = Order.Create(orderId, location).Value;
        var courier = Courier.Create("Ivan", "Tesla", 3, Location.CreateRandom()).Value;
        
        order.Assign(courier);
        order.Complete();
        
        var actual = order.Complete();
        
        actual.IsSuccess.Should().BeFalse();
        actual.Error.Should().Be(Errors.CannotCompleteNotAssignedOrder);
    }

    [Fact]
    public void BeCompletedWhenStatusIsAssigned()
    {
        var location = Location.CreateRandom();
        var orderId = Guid.NewGuid();
        var order = Order.Create(orderId, location).Value;
        var courier = Courier.Create("Ivan", "Tesla", 3, Location.CreateRandom()).Value;
        
        order.Assign(courier);
        
        var actual = order.Complete();
        
        actual.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Completed);
    }
}
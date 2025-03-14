using System;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.CourierAggregate;

public class CourierShould
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NotBeCreatedWhenNameIsInvalid(string name)
    {
        var location = Location.CreateRandom();
        
        var actual = Courier.Create(name, "Tesla", 3, location);
        
        actual.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void NotBeCreatedWhenLocationIsNull()
    {
        var actual = Courier.Create("Ivan", "Tesla", 3, null);
        
        actual.IsSuccess.Should().BeFalse();
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NotBeCreatedWhenTransportNameIsInvalid(string transportName)
    {
        var location = Location.CreateRandom();
        
        var actual = Courier.Create("Ivan", transportName, 3, location);
        
        actual.IsSuccess.Should().BeFalse();
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(4)]
    public void NotBeCreatedWhenTransportSpeedIsInvalid(int transportSpeed)
    {
        var location = Location.CreateRandom();
        
        var actual = Courier.Create("Ivan", "Tesla", transportSpeed, location);
        
        actual.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void BeCreatedWhenDataIsValid()
    {
        var location = Location.CreateRandom();
        
        var actual = Courier.Create("Ivan", "Tesla", 3, location);
        
        actual.IsSuccess.Should().BeTrue();
        actual.Value.Id.Should().NotBe(Guid.Empty);
        actual.Value.Name.Should().Be("Ivan");
        actual.Value.Transport.Name.Should().Be("Tesla");
        actual.Value.Transport.Speed.Should().Be(3);
        actual.Value.Location.Should().Be(location);
        actual.Value.Status.Should().Be(CourierStatus.Free);
    }

    [Fact]
    public void NotSetBusyWhenCourierAlreadyIsBusy()
    {
        var location = Location.CreateRandom();
        var courier = Courier.Create("Ivan", "Tesla", 3, location).Value;

        courier.SetBusy();
        
        var actual = courier.SetBusy();
        
        actual.IsSuccess.Should().BeFalse();
        actual.Error.Should().Be(Errors.CourierIsBusy);
    }
    
    [Fact]
    public void SetBusyWhenCourierIsFree()
    {
        var location = Location.CreateRandom();
        var courier = Courier.Create("Ivan", "Tesla", 3, location).Value;
        
        var actual = courier.SetBusy();
        
        actual.IsSuccess.Should().BeTrue();
        courier.Status.Should().Be(CourierStatus.Busy);
    }

    [Fact]
    public void SetFree()
    {
        var location = Location.CreateRandom();
        var courier = Courier.Create("Ivan", "Tesla", 3, location).Value;
        
        courier.SetBusy();
        courier.SetFree();
        
        courier.Status.Should().Be(CourierStatus.Free);
    }
    
    [Fact]
    public void NotMoveWhenDestinationIsNull()
    {
        var location = Location.CreateRandom();
        var courier = Courier.Create("Ivan", "Tesla", 3, location).Value;

        var actual = courier.Move(null);
        
        actual.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void MoveWhenDestinationIsValid()
    {
        var location = Location.Create(1, 1).Value;
        var courier = Courier.Create("Ivan", "Tesla", 3, location).Value;
        var destination = Location.Create(2, 2).Value;
        
        var actual = courier.Move(destination);
        
        actual.IsSuccess.Should().BeTrue();
        courier.Location.Should().Be(destination);
    }
}
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.CourierAggregate;

public class TransportShould
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ShouldNotBeCreateWhenNameIsInvalid(string name)
    {
        var actual = Transport.Create(name, 1);
        
        actual.IsSuccess.Should().BeFalse();
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(4)]
    public void ShouldNotBeCreateWhenSpeedIsInvalid(int speed)
    {
        var actual = Transport.Create("Tesla Model X", speed);
        
        actual.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void ShouldBeCreateWhenNameAndSpeedAreValid()
    {
        var actual = Transport.Create("Tesla Model X", 3);
        
        actual.IsSuccess.Should().BeTrue();
        actual.Value.Name.Should().Be("Tesla Model X");
        actual.Value.Speed.Should().Be(3);
    }
    
    [Fact]
    public void ShouldReturnErrorWhenFromLocationIsNull()
    {
        var transport = Transport.Create("Tesla Model X", 3).Value;
        var to = Location.CreateRandom();

        var actual = transport.Move(null, to);
        
        actual.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void ShouldReturnErrorWhenToLocationIsNull()
    {
        var transport = Transport.Create("Tesla Model X", 3).Value;
        var from = Location.CreateRandom();

        var actual = transport.Move(from, null);
        
        actual.IsSuccess.Should().BeFalse();
    }
    
    [Fact]
    public void ShouldBeMovedToEndLocationWhenToAndFromLocationAreSame()
    {
        var transport = Transport.Create("Tesla Model X", 3).Value;
        var from = Location.Create(5, 5).Value;
        var to = Location.Create(5, 5).Value;

        var actual = transport.Move(from, to);
        
        actual.IsSuccess.Should().BeTrue();
        actual.Value.Should().Be(to);
    }
    
    [Fact]
    public void ShouldBeMovedToEndLocationWhenSpeedMoreThanDistance()
    {
        var transport = Transport.Create("Tesla Model X", 3).Value;
        var from = Location.Create(1, 1).Value;
        var to = Location.Create(2, 2).Value;

        var actual = transport.Move(from, to);
        
        actual.IsSuccess.Should().BeTrue();
        actual.Value.Should().Be(to);
    }
    
    [Fact]
    public void ShouldBeMovedToEndLocationWhenSpeedEqualsToDistance()
    {
        var transport = Transport.Create("Tesla Model X", 3).Value;
        var from = Location.Create(1, 1).Value;
        var to = Location.Create(3, 2).Value;

        var actual = transport.Move(from, to);
        
        actual.IsSuccess.Should().BeTrue();
        actual.Value.Should().Be(to);
    }
    
    [Fact]
    public void ShouldBeMovedHorizontally()
    {
        var transport = Transport.Create("Tesla Model X", 3).Value;
        var from = Location.Create(1, 1).Value;
        var to = Location.Create(4, 1).Value;

        var actual = transport.Move(from, to); actual.IsSuccess.Should().BeTrue();
        
        actual.IsSuccess.Should().BeTrue();
        actual.Value.Should().Be(Location.Create(4, 1).Value);
    }
    
    [Fact]
    public void ShouldBeMovedVertically()
    {
        var transport = Transport.Create("Tesla Model X", 3).Value;
        var from = Location.Create(5, 1).Value;
        var to = Location.Create(5, 5).Value;

        var actual = transport.Move(from, to); actual.IsSuccess.Should().BeTrue();
        
        actual.IsSuccess.Should().BeTrue();
        actual.Value.Should().Be(Location.Create(5, 4).Value);
    }
    
    [Fact]
    public void ShouldBeMovedHorizontallyAfterVertically()
    {
        var transport = Transport.Create("Tesla Model X", 3).Value;
        var from = Location.Create(3, 3).Value;
        var to = Location.Create(5, 5).Value;

        var actual = transport.Move(from, to); actual.IsSuccess.Should().BeTrue();
        
        actual.IsSuccess.Should().BeTrue();
        actual.Value.Should().Be(Location.Create(5, 4).Value);
    }
    
    [Fact]
    public void ShouldBeMovedVerticallyAfterHorizontally()
    {
        var transport = Transport.Create("Tesla Model X", 3).Value;
        var from = Location.Create(5, 5).Value;
        var to = Location.Create(3, 3).Value;

        var actual = transport.Move(from, to); actual.IsSuccess.Should().BeTrue();
        
        actual.IsSuccess.Should().BeTrue();
        actual.Value.Should().Be(Location.Create(3, 4).Value);
    }
}
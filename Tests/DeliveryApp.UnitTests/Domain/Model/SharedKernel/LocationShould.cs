using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.SharedKernel;

public class LocationShould
{
    [Theory]
    [InlineData(-1, 1)]
    [InlineData(0, 1)]
    [InlineData(11, 1)]
    [InlineData(1, -1)]
    [InlineData(1, 0)]
    [InlineData(1, 11)]
    public void NotBeCreatedWhenCoordinatesAreInvalid(int x, int y)
    {
        var actual = Location.Create(x, y);

        actual.IsSuccess.Should().BeFalse();
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(10, 10)]
    public void BeCreatedWhenCoordinatesAreValid(int x, int y)
    {
        var actual = Location.Create(x, y);

        actual.IsSuccess.Should().BeTrue();
        actual.Value.X.Should().Be(x);
        actual.Value.Y.Should().Be(y);
    }

    [Fact]
    public void BeCreatedWithRandomCoordinates()
    {
        var actual = Location.CreateRandom();

        actual.X.Should().BeGreaterThan(0);
        actual.X.Should().BeLessThan(11);
        actual.Y.Should().BeGreaterThan(0);
        actual.Y.Should().BeLessThan(11);
    }

    [Fact]
    public void ReturnErrorWhenOtherLocationIsNull()
    {
        var location = Location.CreateRandom();

        var actual = location.DistanceTo(null);

        actual.IsSuccess.Should().BeFalse();
    }
    
    [Fact]
    public void ReturnZeroDistanceWhenLocationAreEqual()
    {
        var firstLocation = Location.Create(5, 5).Value;
        var secondLocation = Location.Create(5, 5).Value;

        var actual = firstLocation.DistanceTo(secondLocation);

        actual.IsSuccess.Should().BeTrue();
        actual.Value.Should().Be(0);
    }
    
    [Fact]
    public void ReturnCorrectDistanceWhenLocationAreNotEqual()
    {
        var firstLocation = Location.Create(1, 1).Value;
        var secondLocation = Location.Create(10, 10).Value;

        var actual = firstLocation.DistanceTo(secondLocation);

        actual.IsSuccess.Should().BeTrue();
        actual.Value.Should().Be(18);
    }

    [Fact]
    public void BeNotEqualWhenCoordinatesAreNotEqual()
    {
        var firstLocation = Location.Create(1, 1).Value;
        var secondLocation = Location.Create(1, 10).Value;

        var actual = firstLocation == secondLocation;

        actual.Should().BeFalse();
    }
    
    [Fact]
    public void BeEqualWhenCoordinatesAreEqual()
    {
        var firstLocation = Location.Create(1, 1).Value;
        var secondLocation = Location.Create(1, 1).Value;

        var actual = firstLocation == secondLocation;

        actual.Should().BeTrue();
    }
}
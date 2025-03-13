using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;

namespace DeliveryApp.BDDTests.Domain.Model.CourierAggragete;

public class TransportMoveTests : FeatureFixture
{
    private Transport _transport;
    private Location _currentLocation;
    private Location _finishLocation;
    
    [Scenario]
    public void MoveTransportFrom1_1To8_8AndBack()
    {
        Runner.RunScenario(
            _ => GivenTransport("Test Model X", 3),
            _ => GivenCurrentLocation(1, 1),
            _ => GivenFinishLocation(8, 8),
            
            _ => MoveTransportToFinish(),
            _ => ShouldBeIn(4, 1),
            _ => MoveTransportToFinish(),
            _ => ShouldBeIn(7, 1),
            _ => MoveTransportToFinish(),
            _ => ShouldBeIn(8, 3),
            _ => MoveTransportToFinish(),
            _ => ShouldBeIn(8, 6),
            _ => MoveTransportToFinish(),
            _ => ShouldBeIn(8, 8),
            
            _ => GivenFinishLocation(1, 1),
            _ => MoveTransportToFinish(),
            _ => ShouldBeIn(5, 8),
            _ => MoveTransportToFinish(),
            _ => ShouldBeIn(2, 8),
            _ => MoveTransportToFinish(),
            _ => ShouldBeIn(1, 6),
            _ => MoveTransportToFinish(),
            _ => ShouldBeIn(1, 3),
            _ => MoveTransportToFinish(),
            _ => ShouldBeIn(1, 1)
        );
    }
    
    private void GivenTransport(string name, int speed)
    {
        _transport = Transport.Create(name, speed).Value;
    }

    private void GivenCurrentLocation(int x, int y)
    {
        _currentLocation = Location.Create(x, y).Value;
    }

    private void GivenFinishLocation(int x, int y)
    {
        _finishLocation = Location.Create(x, y).Value;
    }

    private void MoveTransportToFinish()
    {
        _currentLocation = _transport.Move(_currentLocation, _finishLocation).Value;
    }

    private void ShouldBeIn(int x, int y)
    {
        _currentLocation.Should().Be(Location.Create(x, y).Value);
    }
}
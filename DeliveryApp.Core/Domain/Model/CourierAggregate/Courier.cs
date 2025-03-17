using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate;

public class Courier : Aggregate<Guid>
{
    public string Name { get; private set; }
    public Transport Transport { get; private set; }
    public Location Location { get; private set; }
    public CourierStatus Status { get; private set; }

    private Courier(string name, Transport transport, Location location) : base(Guid.NewGuid())
    {
        Name = name;
        Transport = transport;
        Location = location;
        Status = CourierStatus.Free;
    }

    public static Result<Courier, Error> Create(
        string name,
        string transportName,
        int transportSpeed,
        Location location)
    {
        if (string.IsNullOrWhiteSpace(name)) return GeneralErrors.ValueIsRequired(nameof(name));
        if (location is null) return GeneralErrors.ValueIsRequired(nameof(location));

        var (_, isFailure, transport, error) = Transport.Create(transportName, transportSpeed);

        if (isFailure) return error;
        
        return new Courier(name, transport, location);
    }

    public UnitResult<Error> SetBusy()
    {
        if (Status == CourierStatus.Busy) return Errors.CourierIsBusy;
        
        Status = CourierStatus.Busy;

        return UnitResult.Success<Error>();
    }

    public void SetFree()
    {
        Status = CourierStatus.Free;
    }

    public UnitResult<Error> Move(Location destination)
    {
        if (destination is null) return GeneralErrors.ValueIsRequired(nameof(destination));
        
        var (_, isFailure, newLocation, error) = Transport.Move(Location, destination);
        
        if (isFailure) return error;
        
        Location = newLocation;
        
        return UnitResult.Success<Error>();
    }

    public Result<int, Error> CalculateTimeToLocation(Location destination)
    {
        if (destination is null) return GeneralErrors.ValueIsRequired(nameof(destination));

        var (_, isFailure, distance, error) = Location.DistanceTo(destination);

        if (isFailure) return error;

        var time = (double)distance / Transport.Speed;

        return (int)Math.Ceiling(time);
    }
}

public static class Errors
{
    public static Error CourierIsBusy => new(
        $"{nameof(Courier).ToLowerInvariant()}.courier.already.is.busy",
        "Курьер уже занят"
    );
}
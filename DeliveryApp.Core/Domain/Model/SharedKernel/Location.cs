using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.SharedKernel;

public class Location : ValueObject
{
    public int X { get; }
    public int Y { get; }
    
    private Location() { }

    private Location(int x, int y): this()
    {
        X = x;
        Y = y;
    }

    public static Result<Location, Error> Create(int x, int y)
    {
        if (x is < 1 or > 10) return GeneralErrors.ValueIsInvalid(nameof(x));
        if (y is < 1 or > 10) return GeneralErrors.ValueIsInvalid(nameof(y));

        return new Location(x, y);
    }

    public static Location CreateRandom()
    {
        var random = new Random();

        var x = random.Next(1, 11);
        var y = random.Next(1, 11);

        return new Location(x, y);
    }

    public Result<int, Error> DistanceTo(Location other)
    {
        if (other is null) return GeneralErrors.ValueIsInvalid(nameof(other));

        return Math.Abs(other.X - X) + Math.Abs(other.Y - Y);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
    }
}
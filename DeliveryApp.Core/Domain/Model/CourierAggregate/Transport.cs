using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate;

public class Transport : Entity<Guid>
{
    public string Name { get; private set; }
    public int Speed { get; private set; }

    private const int MinSpeed = 1;
    private const int MaxSpeed = 3;

    private Transport(string name, int speed) : base(Guid.NewGuid())
    {
        Name = name;
        Speed = speed;
    }

    public static Result<Transport, Error> Create(string name, int speed)
    {
        if (string.IsNullOrWhiteSpace(name)) return GeneralErrors.ValueIsRequired(nameof(name));
        if (speed is < MinSpeed or > MaxSpeed) return GeneralErrors.ValueIsInvalid(nameof(speed));

        return new Transport(name, speed);
    }
    
    /// <remarks>Двигается в приоритете по оси OX, затем по OY</remarks>
    public Result<Location, Error> Move(Location from, Location to)
    {
        if (from is null) return GeneralErrors.ValueIsRequired(nameof(from));
        if (to is null) return GeneralErrors.ValueIsRequired(nameof(to));
        
        var distance = from.DistanceTo(to).Value;

        // Если расстояние меньше или равно скорости, перемещаемся сразу на целевую локацию
        if (distance <= Speed) return to;

        // Вычисляем разницу по координатам X и Y
        var deltaX = to.X - from.X;
        var deltaY = to.Y - from.Y;

        // Определяем направление движения по каждой из координат
        var stepX = Math.Sign(deltaX); // Направление по X: -1, 0, 1
        var stepY = Math.Sign(deltaY); // Направление по Y: -1, 0, 1

        // Вычисляем количество шагов по каждой из координат
        var stepsX = Math.Min(Math.Abs(deltaX), Speed);
        var stepsY = Math.Min(Math.Abs(deltaY), Speed - stepsX);

        // Вычисляем новые координаты
        var newX = from.X + stepX * stepsX;
        var newY = from.Y + stepY * stepsY;
        
        return Location.Create(newX, newY);
    }
}
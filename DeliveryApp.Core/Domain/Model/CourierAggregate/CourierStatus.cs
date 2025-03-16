using Ardalis.SmartEnum;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate;

public class CourierStatus : SmartEnum<CourierStatus>
{
    public static readonly CourierStatus Free = new(nameof(Free), 1);
    public static readonly CourierStatus Busy = new(nameof(Busy), 2);

    private CourierStatus(string name, int value) : base(name, value) { }
}
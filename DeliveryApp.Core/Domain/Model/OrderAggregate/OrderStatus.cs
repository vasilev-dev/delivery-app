using Ardalis.SmartEnum;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate;

public class OrderStatus : SmartEnum<OrderStatus>
{
    public static readonly OrderStatus Created = new(nameof(Created), 1);
    public static readonly OrderStatus Assigned = new(nameof(Assigned), 2);
    public static readonly OrderStatus Completed = new(nameof(Completed), 3);

    private OrderStatus(string name, int value) : base(name, value) { }
}
namespace Domain;

public static class OrderStateFactory
{
    public static IOrderState Create(OrderStatus status) => status switch
    {
        OrderStatus.Pending      => new PendingState(),
        OrderStatus.Prepared     => new PreparedState(),
        OrderStatus.OnTheWay     => new OnTheWayState(),
        OrderStatus.Delivered    => new DeliveredState(),
        OrderStatus.NotDelivered => new NotDeliveredState(),
        OrderStatus.Cancelled    => new CancelledState(),
        OrderStatus.Delayed      => new DelayedState(),
        _ => throw new InvalidOperationException($"Estado desconocido: {status}")
    };
}

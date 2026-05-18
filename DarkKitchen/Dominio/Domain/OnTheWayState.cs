using Domain.Exceptions;

namespace Domain;

public class OnTheWayState : IOrderState
{
    public IOrderState Prepare() => throw new OrderException("No se puede preparar un pedido que ya esta en camino");
    public IOrderState Cancel() => throw new OrderException("No es posible cancelar un pedido que ya esta en camino");
    public IOrderState Dispatch() => throw new OrderException("El pedido ya se encuentra en camino");
    public IOrderState Deliver() => new DeliveredState();
    public IOrderState Delay() => throw new OrderException("No se puede demorar un pedido que ya esta en camino");
    public OrderStatus GetStatus()
    {
        return OrderStatus.OnTheWay;
    }

    public IOrderState MarkNotDelivered() => new NotDeliveredState();
}

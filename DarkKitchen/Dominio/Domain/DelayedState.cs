using Domain.Exceptions;

namespace Domain;

public class DelayedState : IOrderState
{
    public IOrderState Prepare() => new PreparedState();

    public IOrderState Cancel() => new CancelledState();

    public IOrderState Delay() =>
        throw new OrderException("El pedido ya esta demorado");

    public OrderStatus GetStatus()
    {
        return OrderStatus.Delayed;
    }

    public IOrderState Dispatch() =>
        throw new OrderException("Primero se debe preparar el pedido");

    public IOrderState Deliver() =>
        throw new OrderException("Solo se puede entregar un pedido en camino");

    public IOrderState MarkNotDelivered() =>
        throw new OrderException("Solo se puede marcar como no entregado un pedido en camino");
}

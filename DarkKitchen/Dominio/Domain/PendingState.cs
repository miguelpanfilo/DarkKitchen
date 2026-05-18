using Domain.Exceptions;

namespace Domain;

public class PendingState : IOrderState
{
    public IOrderState Prepare() => new PreparedState();

    public IOrderState Cancel() => new CancelledState();

    public IOrderState Delay() => new DelayedState();
    public OrderStatus GetStatus()
    {
        return OrderStatus.Pending;
    }

    public IOrderState Dispatch() =>
        throw new OrderException("Solo se puede poner en camino un pedido preparado");

    public IOrderState Deliver() =>
        throw new OrderException("Solo se puede entregar un pedido en camino");

    public IOrderState MarkNotDelivered() =>
        throw new OrderException("Solo se puede marcar como no entregado un pedido en camino");
}

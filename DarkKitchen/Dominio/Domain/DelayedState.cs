namespace Domain;

public class DelayedState : IOrderState
{
    public IOrderState Prepare() => new PreparedState();

    public IOrderState Cancel() => new CancelledState();

    public IOrderState Delay() =>
        throw new InvalidOperationException("El pedido ya esta demorado");

    public IOrderState Dispatch() =>
        throw new InvalidOperationException("Primero se debe preparar el pedido");

    public IOrderState Deliver() =>
        throw new InvalidOperationException("Solo se puede entregar un pedido en camino");

    public IOrderState MarkNotDelivered() =>
        throw new InvalidOperationException("Solo se puede marcar como no entregado un pedido en camino");
}

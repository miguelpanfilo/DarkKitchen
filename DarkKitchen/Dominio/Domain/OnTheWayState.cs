namespace Domain;

public class OnTheWayState : IOrderState
{
    public IOrderState Prepare() => throw new InvalidOperationException("No se puede preparar un pedido que ya esta en camino");
    public IOrderState Cancel() => throw new InvalidOperationException("No es posible cancelar un pedido que ya esta en camino");
    public IOrderState Dispatch() => throw new InvalidOperationException("El pedido ya se encuentra en camino");
    public IOrderState Deliver() => new DeliveredState();
    public IOrderState Delay() => throw new InvalidOperationException("No se puede demorar un pedido que ya esta en camino");

    public IOrderState MarkNotDelivered() => new NotDeliveredState();
}

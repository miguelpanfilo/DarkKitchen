using Microsoft.VisualBasic.CompilerServices;

namespace Domain;

public class OnTheWayState : IOrderState
{
    public IOrderState Prepare() => throw new InvalidOperationException();
    public IOrderState Cancel() => throw new InvalidOperationException("No es posible cancelar un pedido que ya esta en camino");
    public IOrderState Dispatch() => throw new InvalidOperationException();
    public IOrderState Deliver() => new DeliveredState();

    public IOrderState MarkNotDelivered() => new NotDeliveredState();
}

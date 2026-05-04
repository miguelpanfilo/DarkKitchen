namespace Domain;

public class CancelledState : IOrderState
{
    public IOrderState Prepare() => throw new InvalidOperationException();
    public IOrderState Cancel() => throw new InvalidOperationException("El pedido ya esta cancelado");
    public IOrderState Dispatch() => throw new InvalidOperationException();
    public IOrderState Deliver() => throw new InvalidOperationException();
    public IOrderState MarkNotDelivered() => throw new InvalidOperationException();
}

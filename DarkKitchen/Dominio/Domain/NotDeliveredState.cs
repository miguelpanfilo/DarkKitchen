namespace Domain;

public class NotDeliveredState : IOrderState
{
    public IOrderState Prepare() => throw new InvalidOperationException();
    public IOrderState Cancel() => throw new InvalidOperationException();
    public IOrderState Dispatch() => throw new InvalidOperationException();
    public IOrderState Deliver() => throw new InvalidOperationException();
    public IOrderState MarkNotDelivered() => throw new InvalidOperationException("El pedido ya se encuentra como no entregado");
    public IOrderState Delay() => throw new InvalidOperationException();
}

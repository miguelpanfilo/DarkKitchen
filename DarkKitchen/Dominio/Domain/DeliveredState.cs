namespace Domain;

public class DeliveredState : IOrderState
{
    public IOrderState Prepare() => throw new InvalidOperationException();
    public IOrderState Cancel() => throw new InvalidOperationException();
    public IOrderState Dispatch() => throw new InvalidOperationException();
    public IOrderState Deliver() => throw new InvalidOperationException();
    public IOrderState MarkNotDelivered() => throw new InvalidOperationException();
    public IOrderState Delay() => throw new InvalidOperationException();
}

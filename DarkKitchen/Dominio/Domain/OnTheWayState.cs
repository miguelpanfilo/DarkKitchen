using Microsoft.VisualBasic.CompilerServices;

namespace Domain;

public class OnTheWayState : IOrderState
{
    public IOrderState Prepare() => throw new InvalidOperationException();
    public IOrderState Cancel() => throw new InvalidOperationException();
    public IOrderState Dispatch() => throw new InvalidOperationException();
    public IOrderState Deliver() => new DeliveredState();

    public IOrderState MarkNotDelivered() => new NotDeliveredState();
}

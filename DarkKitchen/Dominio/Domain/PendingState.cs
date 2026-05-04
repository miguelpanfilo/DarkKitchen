namespace Domain;

public class PendingState : IOrderState
{
    public IOrderState Prepare() => new PreparedState();

    public IOrderState Cancel() => new CancelledState();

    public IOrderState Dispatch() =>
        throw new InvalidOperationException();

    public IOrderState Deliver() =>
        throw new InvalidOperationException();

    public IOrderState MarkNotDelivered() =>
        throw new InvalidOperationException();
}

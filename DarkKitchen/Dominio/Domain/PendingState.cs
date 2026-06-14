namespace Domain;

public sealed class PendingState : OrderStateBase
{
    public override string Name => "Pending";
    public override void Prepare(Order order) => order.TransitionTo(new PreparedState());
    public override void Cancel(Order order) => order.TransitionTo(new CancelledState());
    public override void Delay(Order order) => order.TransitionTo(new DelayedState());
}

namespace Domain;

public sealed class DelayedState : OrderStateBase
{
    public override string Name => "Delayed";
    public override void Prepare(Order order) => order.TransitionTo(new PreparedState());
    public override void Cancel(Order order) => order.TransitionTo(new CancelledState());
}

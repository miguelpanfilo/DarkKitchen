namespace Domain;

public sealed class DelayedState : OrderBase
{
    public override string Name => "Delayed";
    public override void Prepare(Order order) => order.TransitionTo(new PreparedState());
    public override void Cancel(Order order) => order.TransitionTo(new CancelledState());
}

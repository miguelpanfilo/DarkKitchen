namespace Domain;

public sealed class PreparedState : OrderStateBase
{
    public override string Name => "Prepared";
    public override void Dispatch(Order order) => order.TransitionTo(new OnTheWayState());
}

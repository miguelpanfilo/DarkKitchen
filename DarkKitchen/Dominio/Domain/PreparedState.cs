namespace Domain;

public sealed class PreparedState : OrderBaseState
{
    public override string Name => "Prepared";
    public override void Dispatch(Order order) => order.TransitionTo(new OnTheWayState());
}

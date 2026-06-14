namespace Domain;

public sealed class PreparedState : OrderBase
{
    public override string Name => "Prepared";
    public override void Dispatch(Order order) => order.TransitionTo(new OnTheWayState());
}

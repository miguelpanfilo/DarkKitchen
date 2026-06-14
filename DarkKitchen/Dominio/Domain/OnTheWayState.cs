namespace Domain;

public sealed class OnTheWayState : OrderStateBase
{
    public override string Name => "OnTheWay";
    public override void Deliver(Order order) => order.TransitionTo(new DeliveredState());
    public override void MarkNotDelivered(Order order) => order.TransitionTo(new NotDeliveredState());
}

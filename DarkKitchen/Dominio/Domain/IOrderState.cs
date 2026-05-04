namespace Domain;

public interface IOrderState
{
    IOrderState Prepare();
    IOrderState Cancel();
    IOrderState Dispatch();
    IOrderState Deliver();
    IOrderState MarkNotDelivered();
    IOrderState Delay();
}

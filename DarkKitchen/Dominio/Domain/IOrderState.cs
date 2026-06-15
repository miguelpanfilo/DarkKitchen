namespace Domain;

public interface IOrderState
{
    string Name { get; }
    void Prepare(Order order);
    void Cancel(Order order);
    void Dispatch(Order order);
    void Deliver(Order order);
    void MarkNotDelivered(Order order);
    void Delay(Order order);
}

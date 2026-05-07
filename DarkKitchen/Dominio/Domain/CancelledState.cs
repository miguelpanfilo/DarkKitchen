namespace Domain;

public class CancelledState : IOrderState
{
    private const string ErrorMessage = "No se puede modificar un pedido cancelado";

    public IOrderState Prepare() =>
        throw new InvalidOperationException(ErrorMessage);

    public IOrderState Cancel() =>
        throw new InvalidOperationException("El pedido ya esta cancelado");

    public IOrderState Dispatch() =>
        throw new InvalidOperationException(ErrorMessage);

    public IOrderState Deliver() =>
        throw new InvalidOperationException(ErrorMessage);

    public IOrderState MarkNotDelivered() =>
        throw new InvalidOperationException(ErrorMessage);

    public IOrderState Delay() =>
        throw new InvalidOperationException(ErrorMessage);

    public OrderStatus GetStatus()
    {
        return OrderStatus.Cancelled;
    }
}

namespace Domain;

public class DeliveredState : IOrderState
{
    private const string ErrorMessage = "No se puede modificar un pedido entregado";

    public IOrderState Prepare() =>
        throw new InvalidOperationException(ErrorMessage);

    public IOrderState Cancel() =>
        throw new InvalidOperationException(ErrorMessage);

    public IOrderState Dispatch() =>
        throw new InvalidOperationException(ErrorMessage);

    public IOrderState Deliver() =>
        throw new InvalidOperationException("El pedido ya se encuentra entregado");

    public IOrderState MarkNotDelivered() =>
        throw new InvalidOperationException(ErrorMessage);

    public IOrderState Delay() =>
        throw new InvalidOperationException(ErrorMessage);

    public OrderStatus GetStatus()
    {
        return OrderStatus.Delivered;
    }
}

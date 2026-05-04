namespace Domain;

public class NotDeliveredState : IOrderState
{
    private const string ErrorMessage = "No se puede modificar un pedido no entregado";

    public IOrderState Prepare() =>
        throw new InvalidOperationException(ErrorMessage);

    public IOrderState Cancel() =>
        throw new InvalidOperationException(ErrorMessage);

    public IOrderState Dispatch() =>
        throw new InvalidOperationException(ErrorMessage);

    public IOrderState Deliver() =>
        throw new InvalidOperationException(ErrorMessage);

    public IOrderState MarkNotDelivered() =>
        throw new InvalidOperationException("El pedido ya se encuentra como no entregado");

    public IOrderState Delay() =>
        throw new InvalidOperationException(ErrorMessage);
}

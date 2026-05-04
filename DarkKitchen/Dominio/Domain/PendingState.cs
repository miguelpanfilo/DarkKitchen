namespace Domain;

public class PendingState : IOrderState
{
    public IOrderState Prepare() => new PreparedState();

    public IOrderState Cancel() =>
        throw new InvalidOperationException("No se puede cancelar un pedido en estado Pending");

    public IOrderState Dispatch() =>
        throw new InvalidOperationException("No se puede despachar un pedido en estado Pending");

    public IOrderState Deliver() =>
        throw new InvalidOperationException("No se puede entregar un pedido en estado Pending");

    public IOrderState MarkNotDelivered() =>
        throw new InvalidOperationException("No aplica para Pending");
}

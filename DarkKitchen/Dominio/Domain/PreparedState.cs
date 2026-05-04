namespace Domain;

public class PreparedState : IOrderState
{
    public IOrderState Prepare() =>
        throw new InvalidOperationException("El pedido ya está preparado");

    public IOrderState Cancel() =>
        throw new InvalidOperationException("No se puede cancelar un pedido preparado");

    public IOrderState Dispatch() => new OnTheWayState();

    public IOrderState Deliver() =>
        throw new InvalidOperationException("Primero hay que despachar el pedido");

    public IOrderState MarkNotDelivered() =>
        throw new InvalidOperationException("No aplica para Prepared");
}

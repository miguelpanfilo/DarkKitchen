namespace Domain;

public class PreparedState : IOrderState
{
    public IOrderState Prepare() =>
        throw new InvalidOperationException("El pedido ya se encuentra preparado");

    public IOrderState Cancel() =>
        throw new InvalidOperationException("Solo se puede cancelar un pedido pendiente");

    public IOrderState Dispatch() => new OnTheWayState();

    public IOrderState Deliver() =>
        throw new InvalidOperationException("Primero hay que despachar el pedido");

    public IOrderState MarkNotDelivered() =>
        throw new InvalidOperationException("No aplica para Prepared");
    public IOrderState Delay() =>
        throw new InvalidOperationException("Solo se puede demorar un pedido pendiente");
}

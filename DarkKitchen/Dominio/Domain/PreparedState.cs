using Domain.Exceptions;

namespace Domain;

public class PreparedState : IOrderState
{
    public IOrderState Prepare() =>
        throw new OrderException("El pedido ya se encuentra preparado");

    public IOrderState Cancel() =>
        throw new OrderException("Solo se puede cancelar un pedido pendiente o demorado");

    public IOrderState Dispatch() => new OnTheWayState();

    public IOrderState Deliver() =>
        throw new OrderException("Primero hay que despachar el pedido");

    public IOrderState MarkNotDelivered() =>
        throw new OrderException("No aplica para Prepared");
    public IOrderState Delay() =>
        throw new OrderException("Solo se puede demorar un pedido pendiente");

    public OrderStatus GetStatus()
    {
        return OrderStatus.Prepared;
    }
}

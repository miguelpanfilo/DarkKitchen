using Domain.Exceptions;

namespace Domain;

public class CancelledState : IOrderState
{
    private const string ErrorMessage = "No se puede modificar un pedido cancelado";

    public IOrderState Prepare() =>
        throw new OrderException(ErrorMessage);

    public IOrderState Cancel() =>
        throw new OrderException("El pedido ya esta cancelado");

    public IOrderState Dispatch() =>
        throw new OrderException(ErrorMessage);

    public IOrderState Deliver() =>
        throw new OrderException(ErrorMessage);

    public IOrderState MarkNotDelivered() =>
        throw new OrderException(ErrorMessage);

    public IOrderState Delay() =>
        throw new OrderException(ErrorMessage);

    public OrderStatus GetStatus()
    {
        return OrderStatus.Cancelled;
    }
}

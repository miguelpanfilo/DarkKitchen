using Domain.Exceptions;

namespace Domain;

public class NotDeliveredState : IOrderState
{
    private const string ErrorMessage = "No se puede modificar un pedido no entregado";

    public IOrderState Prepare() =>
        throw new OrderException(ErrorMessage);

    public IOrderState Cancel() =>
        throw new OrderException(ErrorMessage);

    public IOrderState Dispatch() =>
        throw new OrderException(ErrorMessage);

    public IOrderState Deliver() =>
        throw new OrderException(ErrorMessage);

    public IOrderState MarkNotDelivered() =>
        throw new OrderException("El pedido ya se encuentra como no entregado");

    public IOrderState Delay() =>
        throw new OrderException(ErrorMessage);

    public OrderStatus GetStatus()
    {
        return OrderStatus.NotDelivered;
    }
}

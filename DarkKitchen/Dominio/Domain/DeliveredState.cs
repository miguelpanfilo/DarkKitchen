using Domain.Exceptions;

namespace Domain;

public class DeliveredState : IOrderState
{
    private const string ErrorMessage = "No se puede modificar un pedido entregado";

    public IOrderState Prepare() =>
        throw new OrderException(ErrorMessage);

    public IOrderState Cancel() =>
        throw new OrderException(ErrorMessage);

    public IOrderState Dispatch() =>
        throw new OrderException(ErrorMessage);

    public IOrderState Deliver() =>
        throw new OrderException("El pedido ya se encuentra entregado");

    public IOrderState MarkNotDelivered() =>
        throw new OrderException(ErrorMessage);

    public IOrderState Delay() =>
        throw new OrderException(ErrorMessage);

    public OrderStatus GetStatus()
    {
        return OrderStatus.Delivered;
    }
}

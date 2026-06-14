using Domain.Exceptions;

namespace Domain;

public abstract class OrderBaseState : IOrderState
{
    public abstract string Name { get; }
    public virtual void Prepare(Order order) => throw new OrderException("Transición no permitida en el estado actual");
    public virtual void Cancel(Order order) => throw new OrderException("Transición no permitida en el estado actual");
    public virtual void Dispatch(Order order) => throw new OrderException("Transición no permitida en el estado actual");
    public virtual void Deliver(Order order) => throw new OrderException("Transición no permitida en el estado actual");
    public virtual void MarkNotDelivered(Order order) => throw new OrderException("Transición no permitida en el estado actual");
    public virtual void Delay(Order order) => throw new OrderException("Transición no permitida en el estado actual");
}

using Domain.Exceptions;

namespace Domain;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; private set; } = string.Empty;
    public int ClientId { get; private set; }
    public User? Client { get; private set; }
    public string DeliveryType { get; private set; } = string.Empty;
    public Address Address { get; private set; } = null!;
    public List<OrderItem> Items { get; private set; } = [];
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime StatusUpdatedAt { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal ShippingCost { get; private set; }
    public decimal Total { get; private set; }
    private IOrderState? _state;

    private Order()
    {
    }

    private IOrderState CurrentState => _state ?? OrderStateFactory.Create(Status);

    public static Order MakeOrder(
        int clientId,
        string deliveryType,
        Address address,
        List<OrderItemData> items,
        decimal shippingCost)
    {
        if(items.Count == 0)
        {
            throw new OrderException("El pedido debe tener al menos un producto");
        }

        var order = new Order
        {
            ClientId = clientId,
            DeliveryType = deliveryType,
            Address = address,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.Now,
            StatusUpdatedAt = DateTime.Now,
            ShippingCost = shippingCost,
            OrderNumber = GenerateOrderNumber(),
            Items = BuildItems(items)
        };

        order._state = new PendingState();
        order.CalculateTotals();

        return order;
    }

    private static List<OrderItem> BuildItems(List<OrderItemData> items)
    {
        var orderItems = new List<OrderItem>();

        foreach (var item in items)
        {
            var orderItem = OrderItem.MakeOrderItem(
                item.ProductId,
                item.Quantity,
                item.UnitPrice,
                item.DiscountApplied);

            orderItems.Add(orderItem);
        }

        return orderItems;
    }

    public void Prepare() => CurrentState.Prepare(this);
    public void Cancel() => CurrentState.Cancel(this);
    public void Dispatch() => CurrentState.Dispatch(this);
    public void Deliver() => CurrentState.Deliver(this);
    public void Delay() => CurrentState.Delay(this);
    public void MarkNotDelivered() => CurrentState.MarkNotDelivered(this);

    private void CalculateTotals()
    {
        Subtotal = Items.Sum(i => i.LineTotal);
        Total = (Subtotal + ShippingCost) * TaxPolicy.VatRate;
    }

    internal void TransitionTo(IOrderState nextState)
    {
        _state = nextState;
        Status = Enum.Parse<OrderStatus>(nextState.Name);
        StatusUpdatedAt = DateTime.Now;
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{Guid.NewGuid()}";
    }
}

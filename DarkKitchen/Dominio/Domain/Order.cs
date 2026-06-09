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
    private Order()
    {
    }

    private IOrderState CurrentState => OrderStateFactory.Create(Status);

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
            CreatedAt = DateTime.UtcNow,
            StatusUpdatedAt = DateTime.UtcNow,
            ShippingCost = shippingCost,
            OrderNumber = GenerateOrderNumber(),
            Items = BuildItems(items)
        };

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

    public void Prepare()
    {
        var nextState = CurrentState.Prepare();
        TransitionTo(nextState.GetStatus());
    }

    public void Cancel()
    {
        var nextState = CurrentState.Cancel();
        TransitionTo(nextState.GetStatus());
    }

    public void Dispatch()
    {
        var nextState = CurrentState.Dispatch();
        TransitionTo(nextState.GetStatus());
    }

    public void Deliver()
    {
        var nextState = CurrentState.Deliver();
        TransitionTo(nextState.GetStatus());
    }

    public void Delay()
    {
        var nextState = CurrentState.Delay();
        TransitionTo(nextState.GetStatus());
    }

    public void MarkNotDelivered()
    {
        var nextState = CurrentState.MarkNotDelivered();

        TransitionTo(nextState.GetStatus());
    }

    private void CalculateTotals()
    {
        Subtotal = Items.Sum(i => i.LineTotal);
        Total = (Subtotal + ShippingCost) * TaxPolicy.VatRate;
    }

    private void TransitionTo(OrderStatus newStatus)
    {
        Status = newStatus;
        StatusUpdatedAt = DateTime.UtcNow;
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{Guid.NewGuid()}";
    }
}

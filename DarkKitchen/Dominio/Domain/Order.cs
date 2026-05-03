using Domain.Exceptions;

namespace Domain;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; private set; } = string.Empty;
    public int ClientId { get; private set; }
    public User? Client { get; private set; }
    public DeliveryType DeliveryType { get; private set; }
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

    public static Order MakeOrder(
        int clientId,
        DeliveryType deliveryType,
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
            OrderNumber = GenerateOrderNumber()
        };

        order.Items = items
            .Select(i => OrderItem.MakeOrderItem(
                i.ProductId,
                i.Quantity,
                i.UnitPrice,
                i.DiscountApplied))
            .ToList();

        order.CalculateTotals();

        return order;
    }

    public void Prepare()
    {
        EnsureCurrentStatus(OrderStatus.Pending);
        TransitionTo(OrderStatus.Prepared);
    }

    public void Cancel()
    {
        EnsureCurrentStatus(OrderStatus.Pending);
        TransitionTo(OrderStatus.Cancelled);
    }

    public void Dispatch()
    {
        EnsureCurrentStatus(OrderStatus.Prepared);
        TransitionTo(OrderStatus.OnTheWay);
    }

    public void Deliver()
    {
        EnsureCurrentStatus(OrderStatus.OnTheWay);
        TransitionTo(OrderStatus.Delivered);
    }

    public void MarkNotDelivered()
    {
        EnsureCurrentStatus(OrderStatus.OnTheWay);
        TransitionTo(OrderStatus.NotDelivered);
    }

    private void CalculateTotals()
    {
        const decimal VatRate = 1.22m;
        Subtotal = Items.Sum(i => i.LineTotal);
        Total = (Subtotal + ShippingCost) * VatRate;
    }

    private void EnsureCurrentStatus(OrderStatus required)
    {
        if(Status != required)
        {
            throw new OrderException($"No se puede transicionar desde {Status}. Estado esperado: {required}");
        }
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

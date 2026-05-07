namespace Contract.Orders;

using Domain;

public class CreateOrderData
{
    public int ClientId { get; set; }
    public DeliveryType DeliveryType { get; set; }
    public Address Address { get; set; } = null!;
    public List<ResolvedOrderItemDto> Items { get; set; } = [];
    public decimal ShippingCost { get; set; }
}

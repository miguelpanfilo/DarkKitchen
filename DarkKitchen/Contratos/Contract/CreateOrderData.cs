namespace Contract;

using Domain;

public class CreateOrderData
{
    public int ClientId { get; set; }
    public string DeliveryType { get; set; } = string.Empty;
    public Address Address { get; set; } = null!;
    public List<ResolvedOrderItemDto> Items { get; set; } = [];
    public decimal ShippingCost { get; set; }
}

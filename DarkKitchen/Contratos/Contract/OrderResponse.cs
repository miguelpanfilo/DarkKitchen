namespace Contract;

public class OrderResponse
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public ClientOrderResponse Client { get; set; } = new();
    public string DeliveryType { get; set; } = string.Empty;
    public AddressResponse Address { get; set; } = new();
    public List<OrderItemResponse> Items { get; set; } = [];
    public int ItemsCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime StatusUpdatedAt { get; set; }

    public decimal Subtotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Iva { get; set; }
    public decimal IvaPercentage { get; set; }
    public decimal Total { get; set; }
}

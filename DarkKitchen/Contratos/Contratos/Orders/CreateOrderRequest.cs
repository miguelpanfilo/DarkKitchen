namespace Contratos.Orders;

public class CreateOrderRequest
{
    public string? DeliveryType { get; set; }
    public CreateAddressRequest Address { get; set; } = new();
    public List<CreateOrderItemRequest> Items { get; set; } = [];
}

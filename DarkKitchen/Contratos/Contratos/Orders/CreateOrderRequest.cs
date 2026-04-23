namespace Contratos.Orders;

public class CreateOrderRequest
{
    public DeliveryTypeDto DeliveryType { get; set; }
    public CreateAddressRequest Address { get; set; } = new();
    public List<CreateOrderItemRequest> Items { get; set; } = [];
}

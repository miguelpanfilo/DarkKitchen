using Contratos;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocioAbstracciones;
using Mapeo;

namespace LogicaDeNegocio;

public class OrderFacade(IOrderService orderService, IProductService productService)
{
    private readonly IOrderService _orderService = orderService;
    private readonly IProductService _productService = productService;

    public OrderResponse CreateOrder(int clientId, CreateOrderRequest request)
    {
        var address = Address.MakeAddress(request.Address.Street, request.Address.DoorNumber, request.Address.Apartment);
        var items = ResolveItems(request.Items);
        var shippingCost = ShippingStrategyFactory.Create(request.DeliveryType).Calculate(0m);
        var order = _orderService.CreateOrder(clientId, request.DeliveryType, address, items, shippingCost);
        return order.ToOrderResponse();
    }

    private List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)> ResolveItems(
        List<CreateOrderItemRequest> requestItems)
    {
        return requestItems.Select(i =>
        {
            var product = _productService.GetById(i.ProductId);
            if(!product.IsActive)
            {
                throw new OrderException($"El producto {product.Name} no está disponible");
            }

            return (i.ProductId, i.Quantity, product.Price, 0m);
        }).ToList();
    }

    public List<OrderResponse> GetClientOrders(int clientId, DateTime? dateFrom, DateTime? dateTo, OrderStatus? status)
    {
        return _orderService.GetClientOrders(clientId, dateFrom, dateTo, status)
            .Select(o => o.ToOrderResponse())
            .ToList();
    }

    public OrderResponse GetById(int orderId)
    {
        return _orderService.GetById(orderId).ToOrderResponse();
    }

    public List<OrderResponse> GetOrdersByFilters(DateTime dateFrom, DateTime dateTo, string? street, OrderStatus? status)
    {
        return _orderService.GetOrdersByFilters(dateFrom, dateTo, street, status)
            .Select(o => o.ToOrderResponse())
            .ToList();
    }

    public void Prepare(int orderId) => _orderService.Prepare(orderId);

    public void Cancel(int orderId) => _orderService.Cancel(orderId);

    public void Dispatch(int orderId) => _orderService.Dispatch(orderId);

    public void Deliver(int orderId) => _orderService.Deliver(orderId);

    public void MarkNotDelivered(int orderId) => _orderService.MarkNotDelivered(orderId);
}

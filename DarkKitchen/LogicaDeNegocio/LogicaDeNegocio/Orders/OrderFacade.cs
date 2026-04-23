using AlmacenamientoAbstracciones;
using Contratos.Orders;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocio.Shipping;
using LogicaDeNegocioAbstracciones.Exceptions;
using LogicaDeNegocioAbstracciones.Orders;
using LogicaDeNegocioAbstracciones.Products;
using Mapeo;

namespace LogicaDeNegocio.Orders;

public class OrderFacade(
    IOrderService orderService,
    IProductService productService,
    IRepositoryUser repositoryUser) : IOrderFacade
{
    private readonly IOrderService _orderService = orderService;
    private readonly IProductService _productService = productService;
    private readonly IRepositoryUser _repositoryUser = repositoryUser;

    public OrderResponse CreateOrder(int clientId, CreateOrderRequest request)
    {
        var deliveryType = GetValidDeliveryType(request.DeliveryType);

        ValidateItems(request.Items);

        var address = Address.MakeAddress(
            request.Address.Street,
            request.Address.DoorNumber,
            request.Address.Apartment);

        var items = ResolveItems(request.Items);
        var shippingCost = ShippingStrategyFactory.Create(deliveryType).Calculate(0m);

        var order = _orderService.CreateOrder(
            clientId,
            deliveryType,
            address,
            items,
            shippingCost);

        return ToOrderResponseWithClient(order);
    }

    public OrderResponse GetById(int orderId)
    {
        var order = _orderService.GetById(orderId);
        return ToOrderResponseWithClient(order);
    }

    public List<OrderResponse> GetClientOrders(int clientId, DateTime? dateFrom, DateTime? dateTo, OrderStatusDto? status)
    {
        return _orderService.GetClientOrders(clientId, dateFrom, dateTo, ToOrderStatus(status))
            .Select(ToOrderResponseWithClient)
            .ToList();
    }

    public List<OrderResponse> GetOrdersByFilters(DateTime dateFrom, DateTime dateTo, string? street, OrderStatusDto? status)
    {
        return _orderService.GetOrdersByFilters(dateFrom, dateTo, street, ToOrderStatus(status))
            .Select(ToOrderResponseWithClient)
            .ToList();
    }

    private OrderResponse ToOrderResponseWithClient(Order order)
    {
        var user = _repositoryUser.FindById(order.ClientId);

        if(user == null)
        {
            throw new NotFoundException($"No se encontró el cliente con id {order.ClientId}");
        }

        return order.ToOrderResponse(user);
    }

    private static void ValidateItems(List<CreateOrderItemRequest> requestItems)
    {
        if(requestItems == null || requestItems.Count == 0)
        {
            throw new ValidationException("El pedido debe contener al menos un producto");
        }

        if(requestItems.Any(i => i.Quantity <= 0))
        {
            throw new ValidationException("La cantidad de cada producto debe ser mayor a 0");
        }
    }

    private static DeliveryType GetValidDeliveryType(DeliveryTypeDto deliveryTypeDto)
    {
        return deliveryTypeDto switch
        {
            DeliveryTypeDto.Express => DeliveryType.Express,
            DeliveryTypeDto.TwentyFourHours => DeliveryType.TwentyFourHours,
            _ => throw new ValidationException("El tipo de entrega no es válido")
        };
    }

    private List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)> ResolveItems(
        List<CreateOrderItemRequest> requestItems)
    {
        return requestItems.Select(i =>
        {
            try
            {
                var product = _productService.GetById(i.ProductId);

                if(!product.IsActive)
                {
                    throw new ValidationException($"El producto {product.Name} no está disponible");
                }

                return (i.ProductId, i.Quantity, product.Price, 0m);
            }
            catch(ProductException)
            {
                throw new NotFoundException($"El producto con id {i.ProductId} no existe");
            }
        }).ToList();
    }

    private static OrderStatus? ToOrderStatus(OrderStatusDto? dto) =>
        dto.HasValue ? Enum.Parse<OrderStatus>(dto.Value.ToString()) : null;

    public void Prepare(int orderId) => _orderService.Prepare(orderId);
    public void Cancel(int orderId) => _orderService.Cancel(orderId);
    public void Dispatch(int orderId) => _orderService.Dispatch(orderId);
    public void Deliver(int orderId) => _orderService.Deliver(orderId);
    public void MarkNotDelivered(int orderId) => _orderService.MarkNotDelivered(orderId);
}

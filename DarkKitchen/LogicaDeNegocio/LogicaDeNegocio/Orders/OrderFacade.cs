using AlmacenamientoAbstracciones;
using Contratos.Orders;
using Dominio;
using LogicaDeNegocio.Shipping;
using LogicaDeNegocioAbstracciones.Exceptions;
using LogicaDeNegocioAbstracciones.Orders;
using Mapeo;

namespace LogicaDeNegocio.Orders;

public class OrderFacade(
    IOrderService orderService,
    IOrderItemResolver orderItemResolver,
    IRepositoryUser repositoryUser) : IOrderFacade
{
    private readonly IOrderItemResolver _orderItemResolver = orderItemResolver;
    private readonly IOrderService _orderService = orderService;
    private readonly IRepositoryUser _repositoryUser = repositoryUser;

    public OrderResponse CreateOrder(int clientId, CreateOrderRequest request)
    {
        var deliveryType = GetValidDeliveryType(request.DeliveryType);

        ValidateItems(request.Items);

        var address = Address.MakeAddress(
            request.Address.Street,
            request.Address.DoorNumber,
            request.Address.Apartment);

        var items = _orderItemResolver.Resolve(request.Items);        var shippingCost = ShippingStrategyFactory.Create(deliveryType).Calculate(0m);

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

    private static OrderStatus? ToOrderStatus(OrderStatusDto? dto) =>
        dto.HasValue ? Enum.Parse<OrderStatus>(dto.Value.ToString()) : null;

    public void Prepare(int orderId) => _orderService.Prepare(orderId);
    public void Cancel(int orderId) => _orderService.Cancel(orderId);
    public void Dispatch(int orderId) => _orderService.Dispatch(orderId);
    public void Deliver(int orderId) => _orderService.Deliver(orderId);
    public void MarkNotDelivered(int orderId) => _orderService.MarkNotDelivered(orderId);
}

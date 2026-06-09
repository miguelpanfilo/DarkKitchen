using BusinessLogicsAbstractions;
using Contract;
using Domain;
using Domain.Exceptions;
using Mappers;

namespace BusinessLogics;

public class OrderFacade(
    IOrderService orderService,
    IOrderItemResolver orderItemResolver,
    IUserService userService,
    IShippingTypeService shippingTypeService,
    IProductService productService) : IOrderFacade
{
    private readonly IOrderItemResolver _orderItemResolver = orderItemResolver;
    private readonly IOrderService _orderService = orderService;
    private readonly IUserService _userService = userService;
    private readonly IShippingTypeService _shippingTypeService = shippingTypeService;
    private readonly IProductService _productService = productService;

    private const string DeliveryTypeRequired = "El tipo de entrega es obligatorio";
    private const string DeliveryTypeInvalid = "El tipo de entrega no es válido";
    private const string ItemsRequired = "El pedido debe contener al menos un producto";
    private const string ItemQuantityInvalid = "La cantidad de cada producto debe ser mayor a 0";
    private const string AddressRequired = "La dirección es obligatoria";
    private const string UnauthorizedAction = "El usuario no tiene permisos para ejecutar esta acción";
    public OrderResponse CreateOrder(int clientId, CreateOrderRequest request)
    {
        return ServiceExceptionHandler.ExecuteWithValidationHandling<OrderResponse, OrderException>(() =>
        {
            var shippingType = GetValidShippingType(request.DeliveryType);

            ValidateItems(request.Items);

            var address = BuildAddress(request);

            var resolvedItems = _orderItemResolver.Resolve(request.Items);

            var order = _orderService.CreateOrder(new CreateOrderData
            {
                ClientId = clientId,
                DeliveryType = shippingType.Name,
                Address = address,
                Items = resolvedItems,
                ShippingCost = shippingType.Cost
            });

            return ToOrderResponseWithClient(order);
        });
    }

    private static Address BuildAddress(CreateOrderRequest request)
    {
        if (request.Address == null)
        {
            throw new ValidationException(AddressRequired);
        }

        return Address.MakeAddress(
            request.Address.Street,
            request.Address.DoorNumber,
            request.Address.Apartment);
    }

    public OrderResponse GetById(int orderId, int currentUserId, UserRolesDto currentUserRole)
    {
        var order = _orderService.GetById(orderId);

        if (currentUserRole == UserRolesDto.Client && order.ClientId != currentUserId)
        {
            throw new ForbiddenException(UnauthorizedAction);
        }

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
        var user = _userService.GetByIdIncludingInactive(order.ClientId);

        var productIds = order.Items.Select(i => i.ProductId).Distinct().ToList();
        var productNames = _productService.GetNamesByIds(productIds);

        return order.ToOrderResponse(user, productNames);
    }

    private static void ValidateItems(List<CreateOrderItemRequest> requestItems)
    {
        if (requestItems == null || requestItems.Count == 0)
        {
            throw new ValidationException(ItemsRequired);
        }

        if (requestItems.Any(i => i.Quantity <= 0))
        {
            throw new ValidationException(ItemQuantityInvalid);
        }
    }

    private ShippingTypeResponseDto GetValidShippingType(string? deliveryType)
    {
        if (string.IsNullOrWhiteSpace(deliveryType))
        {
            throw new ValidationException(DeliveryTypeRequired);
        }

        var shippingType = _shippingTypeService.GetByName(deliveryType);

        if (shippingType == null)
        {
            throw new ValidationException(DeliveryTypeInvalid);
        }

        return shippingType;
    }

    private static OrderStatus? ToOrderStatus(OrderStatusDto? dto) =>
        dto.HasValue ? Enum.Parse<OrderStatus>(dto.Value.ToString()) : null;

    public void Prepare(int orderId) =>
        ServiceExceptionHandler.ExecuteWithConflictHandling<OrderException>(
            () => _orderService.Prepare(orderId));

    public void Cancel(int orderId, int currentUserId, UserRolesDto currentUserRole)
    {
        ValidateClientOrderOwnership(orderId, currentUserId, currentUserRole);

        ServiceExceptionHandler.ExecuteWithConflictHandling<OrderException>(
            () => _orderService.Cancel(orderId));
    }

    private void ValidateClientOrderOwnership(int orderId, int currentUserId, UserRolesDto currentUserRole)
    {
        if (currentUserRole != UserRolesDto.Client)
        {
            return;
        }

        var order = _orderService.GetById(orderId);

        if (order.ClientId != currentUserId)
        {
            throw new ForbiddenException(UnauthorizedAction);
        }
    }

    public void Dispatch(int orderId) =>
        ServiceExceptionHandler.ExecuteWithConflictHandling<OrderException>(
            () => _orderService.Dispatch(orderId));

    public void Deliver(int orderId) =>
        ServiceExceptionHandler.ExecuteWithConflictHandling<OrderException>(
            () => _orderService.Deliver(orderId));

    public void MarkNotDelivered(int orderId) =>
        ServiceExceptionHandler.ExecuteWithConflictHandling<OrderException>(
            () => _orderService.MarkNotDelivered(orderId));
    public void Delay(int orderId) =>
        ServiceExceptionHandler.ExecuteWithConflictHandling<OrderException>(
            () => _orderService.Delay(orderId));
}

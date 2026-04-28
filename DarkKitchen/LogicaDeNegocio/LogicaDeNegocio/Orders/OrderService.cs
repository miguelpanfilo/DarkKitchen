using AlmacenamientoAbstracciones;
using Contratos.Orders;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocioAbstracciones.Exceptions;
using LogicaDeNegocioAbstracciones.Orders;

namespace LogicaDeNegocio.Orders;

public class OrderService(IOrderRepository orderRepository) : IOrderService
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    private const string OrderNotFoundMessage = "Pedido no encontrado";

    public Order CreateOrder(
        int clientId,
        DeliveryType deliveryType,
        Address address,
        List<ResolvedOrderItemDto> items,
        decimal shippingCost)
    {
        var domainItems = items
            .Select(i => (i.ProductId, i.Quantity, i.UnitPrice, i.DiscountApplied))
            .ToList();

        var order = Order.MakeOrder(clientId, deliveryType, address, domainItems, shippingCost);

        return _orderRepository.Add(order);
    }

    public Order GetById(int orderId)
    {
        var order = _orderRepository.GetById(orderId);
        if(order is null)
        {
            throw new NotFoundException(OrderNotFoundMessage);
        }

        return order;
    }

    public List<Order> GetClientOrders(int clientId, DateTime? dateFrom, DateTime? dateTo, OrderStatus? status)
    {
        return _orderRepository.GetByClientId(clientId, dateFrom, dateTo, status);
    }

    public List<Order> GetOrdersByFilters(DateTime dateFrom, DateTime dateTo, string? street, OrderStatus? status)
    {
        return _orderRepository.GetByFilters(dateFrom, dateTo, street, status);
    }

    public void Prepare(int orderId) => ApplyStateChange(orderId, o => o.Prepare());

    public void Cancel(int orderId) => ApplyStateChange(orderId, o => o.Cancel());

    public void Dispatch(int orderId) => ApplyStateChange(orderId, o => o.Dispatch());

    public void Deliver(int orderId) => ApplyStateChange(orderId, o => o.Deliver());

    public void MarkNotDelivered(int orderId) => ApplyStateChange(orderId, o => o.MarkNotDelivered());

    private void ApplyStateChange(int orderId, Action<Order> transition)
    {
        var order = GetById(orderId);

        try
        {
            transition(order);
        }
        catch(OrderException ex)
        {
            throw new ConflictException(ex.Message);
        }

        _orderRepository.Update(order);
    }
}

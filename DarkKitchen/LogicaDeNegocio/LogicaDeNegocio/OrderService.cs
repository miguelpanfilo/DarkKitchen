using AlmacenamientoAbstracciones;
using Dominio;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;

namespace LogicaDeNegocio;

public class OrderService(IOrderRepository orderRepository) : IOrderService
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    private const string OrderNotFoundMessage = "Pedido no encontrado";

    public Order CreateOrder(
        int clientId,
        DeliveryType deliveryType,
        Address address,
        List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountApplied)> items,
        decimal shippingCost)
    {
        var order = Order.MakeOrder(clientId, deliveryType, address, items, shippingCost);
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

    public void Prepare(int orderId)
    {
        var order = GetById(orderId);
        order.Prepare();
        _orderRepository.Update(order);
    }

    public void Cancel(int orderId)
    {
        var order = GetById(orderId);
        order.Cancel();
        _orderRepository.Update(order);
    }

    public void Dispatch(int orderId)
    {
        var order = GetById(orderId);
        order.Dispatch();
        _orderRepository.Update(order);
    }

    public void Deliver(int orderId)
    {
        var order = GetById(orderId);
        order.Deliver();
        _orderRepository.Update(order);
    }

    public void MarkNotDelivered(int orderId)
    {
        var order = GetById(orderId);
        order.MarkNotDelivered();
        _orderRepository.Update(order);
    }
}

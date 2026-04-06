using Dominio;

namespace LogicaDeNegocioAbstracciones;

public interface IOrderService
{
    Order CreateOrder(int clientId, DeliveryType deliveryType, Address address,
        List<(int ProductId, int Quantity)> items);

    Order GetById(int orderId, int requestingUserId);

    List<Order> GetClientOrders(int clientId, DateTime? dateFrom, DateTime? dateTo, OrderStatus? status);

    List<Order> GetOrdersByFilters(DateTime dateFrom, DateTime dateTo, string? street, OrderStatus? status);

    void Prepare(int orderId);

    void Cancel(int orderId);

    void Dispatch(int orderId);

    void Deliver(int orderId);

    void MarkNotDelivered(int orderId);
}

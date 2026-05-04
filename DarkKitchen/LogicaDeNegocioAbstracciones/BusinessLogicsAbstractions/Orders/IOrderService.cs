using Contract.Orders;
using Domain;

namespace BusinessLogicsAbstractions.Orders;

public interface IOrderService
{
    Order CreateOrder(CreateOrderData data);

    Order GetById(int orderId);

    List<Order> GetClientOrders(int clientId, DateTime? dateFrom, DateTime? dateTo, OrderStatus? status);

    List<Order> GetOrdersByFilters(DateTime dateFrom, DateTime dateTo, string? street, OrderStatus? status);

    void Prepare(int orderId);

    void Cancel(int orderId);

    void Dispatch(int orderId);

    void Deliver(int orderId);

    void MarkNotDelivered(int orderId);

    void Delay(int orderId);
}

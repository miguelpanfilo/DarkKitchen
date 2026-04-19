using Contratos.Orders;
using Dominio;

namespace LogicaDeNegocioAbstracciones.Orders;

public interface IOrderFacade
{
    OrderResponse CreateOrder(int clientId, CreateOrderRequest request);

    OrderResponse GetById(int orderId);

    List<OrderResponse> GetClientOrders(int clientId, DateTime? dateFrom, DateTime? dateTo, OrderStatus? status);

    List<OrderResponse> GetOrdersByFilters(DateTime dateFrom, DateTime dateTo, string? street, OrderStatus? status);

    void Prepare(int orderId);

    void Cancel(int orderId);

    void Dispatch(int orderId);

    void Deliver(int orderId);

    void MarkNotDelivered(int orderId);
}

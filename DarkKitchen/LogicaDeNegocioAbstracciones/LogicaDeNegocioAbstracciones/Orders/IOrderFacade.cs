using Contratos.Orders;
using Contratos.Reportes;

namespace LogicaDeNegocioAbstracciones.Orders;

public interface IOrderFacade
{
    List<TopProductoReporteItemDto> GetTopProductos(DateTime? fechaDesde, DateTime? fechaHasta);

    ReporteVentasDto GetReporteVentas(DateTime? fechaDesde, DateTime? fechaHasta);

    OrderResponse CreateOrder(int clientId, CreateOrderRequest request);

    OrderResponse GetById(int orderId);

    List<OrderResponse> GetClientOrders(int clientId, DateTime? dateFrom, DateTime? dateTo, OrderStatusDto? status);

    List<OrderResponse> GetOrdersByFilters(DateTime dateFrom, DateTime dateTo, string? street, OrderStatusDto? status);

    void Prepare(int orderId);

    void Cancel(int orderId);

    void Dispatch(int orderId);

    void Deliver(int orderId);

    void MarkNotDelivered(int orderId);
}

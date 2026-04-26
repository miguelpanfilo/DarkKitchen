using AlmacenamientoAbstracciones;
using Contratos.Reports;
using LogicaDeNegocioAbstracciones.Reports;
using Mapeo;

namespace LogicaDeNegocio.Reports;

public class ReportService(IOrderRepository orderRepository) : IReportService
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public List<TopProductReportItemDto> GetTopProducts(DateTime? dateFrom, DateTime? dateTo)
    {
        var from = dateFrom ?? DateTime.MinValue;
        var to = dateTo ?? DateTime.MaxValue;

        var orders = _orderRepository.GetByFilters(from, to, null, null);

        return orders.ToTopProductsReport();
    }

    public SalesReportDto GetSalesReport(DateTime? dateFrom, DateTime? dateTo)
    {
        var from = dateFrom ?? DateTime.MinValue;
        var to = dateTo ?? DateTime.MaxValue;

        var orders = _orderRepository.GetByFilters(from, to, null, null);

        return orders.ToSalesReport();
    }
}

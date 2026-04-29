using AlmacenamientoAbstracciones;
using Contratos.Reports;
using LogicaDeNegocioAbstracciones.Exceptions;
using LogicaDeNegocioAbstracciones.Reports;
using Mapeo;

namespace LogicaDeNegocio.Reports;

public class ReportService(IOrderRepository orderRepository) : IReportService
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public List<TopProductReportItemDto> GetTopProducts(DateTime? dateFrom, DateTime? dateTo)
    {
        ValidateDateRange(dateFrom, dateTo);

        var from = dateFrom ?? DateTime.MinValue;
        var to = dateTo ?? DateTime.MaxValue;

        var orders = _orderRepository.GetByFilters(from, to, null, null);

        return orders.ToTopProductsReport();
    }

    public SalesReportDto GetSalesReport(DateTime? dateFrom, DateTime? dateTo)
    {
        ValidateDateRange(dateFrom, dateTo);

        var from = dateFrom ?? DateTime.MinValue;
        var to = dateTo ?? DateTime.MaxValue;

        var orders = _orderRepository.GetByFilters(from, to, null, null);

        return orders.ToSalesReport();
    }

    public static void ValidateDateRange(DateTime? dateFrom, DateTime? dateTo)
    {
        if(dateFrom.HasValue && dateTo.HasValue && dateFrom > dateTo)
        {
            throw new ValidationException("Fecha inicio no puede ser posterior a fecha fin");
        }
    }
}

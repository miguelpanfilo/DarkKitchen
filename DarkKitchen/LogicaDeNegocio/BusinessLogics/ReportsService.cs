using BusinessLogicsAbstractions;
using Contract.Reports;
using DataAccessAbstractions;
using Domain;
using Mappers;

namespace BusinessLogics;

public class ReportService(IOrderRepository orderRepository) : IReportService
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public List<TopProductReportItemDto> GetTopProducts(DateTime?
        dateFrom, DateTime? dateTo)
    {
        ValidateDateRange(dateFrom, dateTo);

        var from = dateFrom ?? DateTime.MinValue;
        var to = dateTo ?? DateTime.MaxValue;

        var orders = _orderRepository.GetByFilters(from, to, null,
            null);
        var data = new OrderReportData(orders);
        var visitor = new TopProductsReportVisitor();

        data.Accept(visitor);

        return visitor.Result;
    }

    public SalesReportDto GetSalesReport(DateTime? dateFrom, DateTime?
        dateTo)
    {
        ValidateDateRange(dateFrom, dateTo);

        var from = dateFrom ?? DateTime.MinValue;
        var to = dateTo ?? DateTime.MaxValue;

        var orders = _orderRepository.GetByFilters(from, to, null,
            null);
        var data = new OrderReportData(orders);
        var visitor = new SalesReportVisitor();

        data.Accept(visitor);

        return visitor.Result;
    }

    public static void ValidateDateRange(DateTime? dateFrom, DateTime? dateTo)
    {
        if(dateFrom.HasValue && dateTo.HasValue && dateFrom > dateTo)
        {
            throw new ValidationException("Fecha inicio no puede ser posterior a fecha fin");
        }
    }
}

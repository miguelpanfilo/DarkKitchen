using Contract;
using Domain;

namespace BusinessLogicsAbstractions;

public interface IReportService
{
    List<TopProductReportItemDto> GetTopProducts(
        DateTime? dateFrom,
        DateTime? dateTo);

    SalesReportDto GetSalesReport(
        DateTime? dateFrom,
        DateTime? dateTo);

    void ExecuteReport(OrderReportData data, IReportVisitor visitor);
}

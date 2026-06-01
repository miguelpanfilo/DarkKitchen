using Contract;

namespace BusinessLogicsAbstractions;

public interface IReportService
{
    List<TopProductReportItemDto> GetTopProducts(
        DateTime? dateFrom,
        DateTime? dateTo);

    SalesReportDto GetSalesReport(
        DateTime? dateFrom,
        DateTime? dateTo);
}

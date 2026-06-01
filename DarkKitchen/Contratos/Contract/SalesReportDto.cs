namespace Contract;

public class SalesReportDto
{
    public List<SalesPeriodDto> Periods { get; set; } = [];
    public decimal GrandTotal { get; set; }
}

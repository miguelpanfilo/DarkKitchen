namespace Contract.Reports;

public class SalesPeriodDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public List<CustomerSalesDto> Customers { get; set; } = [];
    public decimal MonthTotal { get; set; }
}

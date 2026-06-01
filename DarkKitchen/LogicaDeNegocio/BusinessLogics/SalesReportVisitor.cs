using Contract.Reports;
using Domain;

namespace BusinessLogics;

public class SalesReportVisitor : IReportVisitor
{
    public SalesReportDto Result { get; private set; } = new();

    public void Visit(OrderReportData data)
    {
        var periods = data.Orders
            .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.Month)
            .Select(g => new SalesPeriodDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,

                Customers = g
                    .GroupBy(o => o.ClientId)
                    .Select(cg => new CustomerSalesDto
                    {
                        Customer =
                            $"{cg.First().Client!.Name} " +
                            $"{cg.First().Client!.LastName}",

                        TotalSales = cg.Sum(o => o.Total)
                    })
                    .OrderBy(c => c.Customer)
                    .ToList(),

                MonthTotal = g.Sum(o => o.Total)
            })
            .ToList();

        Result = new SalesReportDto
        {
            Periods = periods,
            GrandTotal = periods.Sum(p => p.MonthTotal)
        };
    }
}

using Contract.Reports;
using Domain;

namespace BusinessLogics;

public class TopProductsReportVisitor : IReportVisitor
{
    public List<TopProductReportItemDto> Result { get; private set; }
        = [];

    public void Visit(OrderReportData data)
    {
        Result = data.Orders
            .SelectMany(o => o.Items)
            .GroupBy(i => i.ProductId)
            .Select(g => new { Product = g.First().Product!, QuantitySold = g.Sum(i => i.Quantity) })
            .OrderByDescending(x => x.QuantitySold)
            .Take(5)
            .Select(x => new TopProductReportItemDto
            {
                Code = x.Product.Code,
                Name = x.Product.Name,
                QuantitySold = x.QuantitySold,
                Images = x.Product.Photos.Select(p => p.Path).ToList()
            })
            .ToList();
    }
}

namespace Contratos.Reports;

public class TopProductReportItemDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public List<string> Images { get; set; } = [];
}

namespace Contratos.Products;

public class ProductSearchRequestDto
{
    public string? CommercialLine { get; init; }
    public List<string>? Categories { get; init; }
    public string? Name { get; init; }
}

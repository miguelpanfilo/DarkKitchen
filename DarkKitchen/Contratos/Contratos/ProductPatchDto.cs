namespace Contratos;

public class ProductPatchDto
{
    public string? Code { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? CommercialLine { get; init; }
    public string? Category { get; init; }
    public decimal? Price { get; init; }
    public List<ProductPatchPhotoDto>? Photos { get; set; }
}

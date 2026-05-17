namespace Contract.Importers;

public class ImportedProductDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string CommercialLine { get; set; } = null!;
    public string Category { get; set; } = null!;
    public List<string> PhotoPaths { get; set; } = [];
}

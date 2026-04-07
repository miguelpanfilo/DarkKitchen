namespace Contratos;

public class ProductDTO
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string CommercialLine { get; set; }
    public string Category { get; set; }

    public ProductDTO(string code, string name, string description, string commercialLine, string category)
    {
        Code = code;
        Name = name;
        Description = description;
        CommercialLine = commercialLine;
        Category = category;
    }
}

using Dominio.Excepciones;

namespace Dominio;

public class Product
{
    public string Code { get; }
    public string Name { get; }
    public string Description { get; }
    public string CommercialLine { get; }
    public string Category { get; }
    public Product(string code, string name, string description, string commercialLine, string category)
    {
        ValidateRequired(code, "El código del producto");
        ValidateRequired(name, "El nombre de producto");
        Code = code;
        Name = name;
        Description = description;
        CommercialLine = commercialLine;
        Category = category;
    }

    private static void ValidateRequired(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ProductException($"{fieldName} no puede estar vacio.");
        }
    }
}

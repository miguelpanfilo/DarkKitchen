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
        ValidateRequired(code, "El código");
        ValidateRequired(name, "El nombre");
        ValidateRequired(description, "La descripcion");
        ValidateRequired(commercialLine, "Línea comercial");
        ValidateRequired(category, "La categoria");
        if(code.Length < 5)
        {
            throw new ProductException("El código debe tener al menos 5 caracteres.");
        }

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
            throw new ProductException($"{fieldName} del producto, no puede estar vacio.");
        }
    }
}

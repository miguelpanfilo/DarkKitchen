using Dominio.Excepciones;

namespace Dominio;

public class Product
{
    public string Code { get; }
    public string Name { get; }
    public string Description { get; }
    public string CommercialLine { get; }
    public string Category { get; }
    private const int CODE_MIN_LENGTH = 5;
    private const int CODE_MAX_LENGTH = 20;
    public Product(string code, string name, string description, string commercialLine, string category)
    {
        ValidateRequired(code, "El código");
        ValidateRequired(name, "El nombre");
        ValidateRequired(description, "La descripcion");
        ValidateRequired(commercialLine, "Línea comercial");
        ValidateRequired(category, "La categoria");
        ValidateLength(code, "El código", CODE_MIN_LENGTH, CODE_MAX_LENGTH);

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
            throw new ProductException($"{fieldName} no puede estar vacío.");
        }
    }

    private static void ValidateLength(string value, string fieldName, int min, int max)
    {
        if(value.Length < min || value.Length > max)
        {
            throw new ProductException($"{fieldName} debe tener entre {min} y {max} caracteres.");
        }
    }
}

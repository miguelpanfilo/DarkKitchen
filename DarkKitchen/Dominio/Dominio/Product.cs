using System.Diagnostics.CodeAnalysis;
using Dominio.Exceptions;

namespace Dominio;

public class Product
{
    public int Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string CommercialLine { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public List<ProductPhoto> Photos { get; private set; } = new();

    public bool IsActive { get; private set; }
    public decimal Price { get; private set; }

    private const int CODE_MIN_LENGTH = 5;
    private const int CODE_MAX_LENGTH = 20;

    private const int NAME_MIN_LENGTH = 10;
    private const int NAME_MAX_LENGTH = 50;

    private const int DESCRIPTION_MIN_LENGTH = 20;
    private const int DESCRIPTION_MAX_LENGTH = 500;

    private const decimal MIN_PRICE = 0;

    private const int MIN_PHOTOS = 1;
    private const int MAX_PHOTOS = 3;

    public static Product MakeProduct(
        string code,
        string name,
        string description,
        string commercialLine,
        string category,
        decimal price,
        List<ProductPhoto> photos)
    {
        return new Product(code, name, description, commercialLine, category, price, photos);
    }

    [ExcludeFromCodeCoverage]
    private Product()
    {
    }

    private Product(
        string code,
        string name,
        string description,
        string commercialLine,
        string category,
        decimal price,
        List<ProductPhoto> photos)
    {
        ValidateRequired(code, "El código");
        ValidateLength(code, "El código", CODE_MIN_LENGTH, CODE_MAX_LENGTH);

        ValidateRequired(name, "El nombre");
        ValidateLength(name, "El nombre", NAME_MIN_LENGTH, NAME_MAX_LENGTH);

        ValidateRequired(description, "La descripción");
        ValidateLength(description, "La descripción", DESCRIPTION_MIN_LENGTH, DESCRIPTION_MAX_LENGTH);

        ValidateRequired(commercialLine, "Línea comercial");
        ValidateRequired(category, "La categoría");

        ValidatePrice(price);

        ValidatePhotos(photos);

        Code = code.Trim();
        Name = name.Trim();
        Description = description.Trim();
        CommercialLine = commercialLine.Trim();
        Category = category.Trim();
        Price = price;
        IsActive = true;
        Photos = photos;
    }

    public void UpdateName(string newName)
    {
        Name = newName.Trim();
    }

    public void UpdateCode(string newCode)
    {
        Code = newCode.Trim();
    }

    public void UpdateDescription(string newDescription)
    {
        Description = newDescription.Trim();
    }

    public void UpdateCommercialLine(string newCommercialLine)
    {
        CommercialLine = newCommercialLine.Trim();
    }

    public void UpdateCategory(string newCategory)
    {
        Category = newCategory.Trim();
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private static void ValidateRequired(string value, string fieldName)
    {
        if(string.IsNullOrWhiteSpace(value))
        {
            throw new ProductException($"{fieldName} no puede estar vacío.");
        }
    }

    private static void ValidateLength(string value, string fieldName, int min, int max)
    {
        var trimmed = value.Trim();

        if(trimmed.Length < min || trimmed.Length > max)
        {
            throw new ProductException($"{fieldName} debe tener entre {min} y {max} caracteres.");
        }
    }

    private static void ValidatePrice(decimal price)
    {
        if(price <= MIN_PRICE)
        {
            throw new ProductException("El precio debe ser mayor a 0.");
        }
    }

    private static void ValidatePhotos(List<ProductPhoto> photos)
    {
        if (photos == null || photos.Count < MIN_PHOTOS || photos.Count > MAX_PHOTOS)
        {
            throw new ProductException("El producto debe tener entre 1 y 3 fotos.");
        }
    }
}

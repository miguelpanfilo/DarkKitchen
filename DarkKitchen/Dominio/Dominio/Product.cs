using Dominio.Exceptions;

namespace Dominio;

public class Product
{
    public int Id { get; set; }
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public bool IsActive { get; private set; }

    private Product() { }

    public static Product MakeProduct(string name, decimal price, bool isActive = true)
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            throw new OrderException("Product name cannot be empty");
        }

        if(price < 0)
        {
            throw new OrderException("Product price cannot be negative");
        }

        return new Product
        {
            Name = name.Trim(),
            Price = price,
            IsActive = isActive
        };
    }
}

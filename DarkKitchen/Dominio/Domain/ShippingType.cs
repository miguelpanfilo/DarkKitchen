using System.Diagnostics.CodeAnalysis;
using Domain.Exceptions;

namespace Domain;

public class ShippingType
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal Cost { get; private set; }

    public static ShippingType MakeShippingType(string name, decimal cost)
    {
        return new ShippingType(name, cost);
    }

    [ExcludeFromCodeCoverage]
    private ShippingType() { }

    private ShippingType(string name, decimal cost)
    {
        ValidateName(name);
        ValidateCost(cost);

        Name = name.Trim();
        Cost = cost;
    }

    public void UpdateName(string name)
    {
        ValidateName(name);
        Name = name.Trim();
    }

    public void UpdateCost(decimal cost)
    {
        ValidateCost(cost);
        Cost = cost;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ShippingTypeException("El nombre del tipo de envío no puede estar vacío.");
        }
    }

    private static void ValidateCost(decimal cost)
    {
        if (cost < 0)
        {
            throw new ShippingTypeException("El costo del tipo de envío no puede ser negativo.");
        }
    }
}

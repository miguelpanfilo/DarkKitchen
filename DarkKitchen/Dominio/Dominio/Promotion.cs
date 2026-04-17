using Dominio.Exceptions;

namespace Dominio;

public class Promotion
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public List<Product> Products { get; private set; } = new();

    private const decimal MinPercentage = 0;
    private const decimal MaxPercentage = 100;

    private Promotion(
        string name,
        decimal discountPercentage,
        DateOnly startDate,
        DateOnly endDate)
    {
        ValidateName(name);

        ValidatePercentage(discountPercentage);

        ValidateDateRange(startDate, endDate);

        Name = name;
        DiscountPercentage = discountPercentage;
        StartDate = startDate;
        EndDate = endDate;
    }

    public static Promotion MakePromotion(
        string name,
        decimal discountPercentage,
        DateOnly startDate,
        DateOnly endDate)
    {
        return new Promotion(name, discountPercentage, startDate, endDate);
    }

    public void AssociateProduct(Product product)
    {
        if(Products.Contains(product))
        {
            throw new PromotionException("El producto ya está asociado a la promoción");
        }

        Products.Add(product);
    }

    public void DisassociateProduct(Product product)
    {
        Products.Remove(product);
    }

    private void ValidateName(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            throw new PromotionException("El nombre de la promoción no puede ser vacío");
        }
    }

    private void ValidatePercentage(decimal discountPercentage)
    {
        if(discountPercentage is <= MinPercentage or > MaxPercentage)
        {
            throw new PromotionException("El porcentaje de descuento debe estar entre 1 y 100");
        }
    }

    private void ValidateDateRange(DateOnly startDate, DateOnly endDate)
    {
        if (startDate > endDate)
        {
            throw new PromotionException("La fecha de inicio no puede ser mayor a la fecha de fin");
        }
    }
}

using Dominio.Exceptions;

namespace Dominio;

public class Promotion
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public decimal DiscountPercentage { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public List<Product> Products { get; private set; } = new();

    private const decimal MinPercentage = 1;
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

    public void UpdateName(string name)
    {
        ValidateName(name);
        Name = name;
    }

    public void UpdateDiscountPercentage(decimal discountPercentage)
    {
        ValidatePercentage(discountPercentage);
        DiscountPercentage = discountPercentage;
    }

    public void UpdateStartDate(DateOnly startDate)
    {
        StartDate = startDate;
    }

    public void UpdateEndDate(DateOnly endDate)
    {
        EndDate = endDate;
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
        if(!Products.Contains(product))
        {
            throw new PromotionException("El producto no está asociado a la promoción");
        }

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
        if(discountPercentage is < MinPercentage or > MaxPercentage)
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

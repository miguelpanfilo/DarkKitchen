using Dominio.Exceptions;

namespace Dominio;

public class Promotion
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public decimal DiscountPercentage { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }

    public List<PromotionProduct> PromotionProducts { get; private set; } = new();
    public bool IsDeleted { get; private set; }

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

    public void Delete()
    {
        if(IsDeleted)
        {
            throw new PromotionException("La promoción ya está eliminada");
        }

        IsDeleted = true;
    }

    public void UpdateName(string name)
    {
        ValidateNotExpired();
        ValidateName(name);
        Name = name;
    }

    public void UpdateDiscountPercentage(decimal discountPercentage)
    {
        ValidateNotExpired();
        ValidatePercentage(discountPercentage);
        DiscountPercentage = discountPercentage;
    }

    public void UpdateDates(DateOnly startDate, DateOnly endDate)
    {
        ValidateDateRange(startDate, endDate);
        StartDate = startDate;
        EndDate = endDate;
    }

    public void AssociateProduct(int productId)
    {
        if(PromotionProducts.Any(pp => pp.ProductId == productId))
        {
            throw new PromotionException("El producto ya está asociado a la promoción");
        }

        PromotionProducts.Add(new PromotionProduct(productId));
    }

    public void DisassociateProduct(int productId)
    {
        var promotionProduct = PromotionProducts.FirstOrDefault(pp => pp.ProductId == productId);

        if(promotionProduct == null)
        {
            throw new PromotionException("El producto no está asociado a la promoción");
        }

        PromotionProducts.Remove(promotionProduct);
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

    private void ValidateNotExpired()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (EndDate < today)
        {
            throw new PromotionException("La promoción expiró, no se puede modificar");
        }
    }
}

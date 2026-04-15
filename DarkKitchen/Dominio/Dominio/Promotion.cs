using Dominio.Exceptions;

namespace Dominio;

public class Promotion
{
    public string Name { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    private Promotion(
        string name,
        decimal discountPercentage,
        DateOnly startDate,
        DateOnly endDate)
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            throw new PromotionException("El nombre de la promoción no puede ser vacío");
        }

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
}

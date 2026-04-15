namespace Dominio;

public class Promotion
{
    public string Name { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public Promotion(
        string name,
        decimal discountPercentage,
        DateOnly startDate,
        DateOnly endDate)
    {
        Name = name;
        DiscountPercentage = discountPercentage;
        StartDate = startDate;
        EndDate = endDate;
    }
}

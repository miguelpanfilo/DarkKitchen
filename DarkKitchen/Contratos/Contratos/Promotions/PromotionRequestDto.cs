namespace Contratos.Promotions;

public class PromotionRequestDto
{
    public string Name { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public PromotionRequestDto(string name, decimal discountPercentage, DateOnly startDate, DateOnly endDate)
    {
        Name = name;
        DiscountPercentage = discountPercentage;
        StartDate = startDate;
        EndDate = endDate;
    }
}

namespace Contratos;

public class PromotionResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal DiscountPercentage { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public PromotionResponseDto(int id, string name, decimal discountPercentage,
        DateOnly startDate, DateOnly endDate)
    {
        Id = id;
        Name = name;
        DiscountPercentage = discountPercentage;
        StartDate = startDate;
        EndDate = endDate;
    }
}

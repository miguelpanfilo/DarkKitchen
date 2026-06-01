namespace Contract;

public class PromotionRequestDto
{
    public string Name { get; set; } = string.Empty;
    public decimal DiscountPercentage { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}

namespace Contratos.Promotions;

public class PromotionPatchDto
{
    public string? Name { get; init; }
    public decimal? DiscountPercentage { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
}

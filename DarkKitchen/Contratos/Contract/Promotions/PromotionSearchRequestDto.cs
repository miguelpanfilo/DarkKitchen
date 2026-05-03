namespace Contract.Promotions;

public class PromotionSearchRequestDto
{
    public DateOnly? Date { get; set; }
    public string? CommercialLine { get; set; }
    public int? ProductId { get; set; }
}

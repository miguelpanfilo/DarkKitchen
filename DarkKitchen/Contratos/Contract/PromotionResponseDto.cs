
namespace Contract;

public class PromotionResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal DiscountPercentage { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public List<ProductSummaryDto> Products { get; set; }

    public PromotionResponseDto(
        int id,
        string name,
        decimal discountPercentage,
        DateOnly startDate,
        DateOnly endDate,
        List<ProductSummaryDto> products)
    {
        Id = id;
        Name = name;
        DiscountPercentage = discountPercentage;
        StartDate = startDate;
        EndDate = endDate;
        Products = products;
    }
}

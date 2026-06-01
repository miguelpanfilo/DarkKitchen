using Contract;
using Domain;

namespace Mappers;

public class PromotionMapper
{
    public static Promotion ToEntity(PromotionRequestDto requestDto)
    {
        return Promotion.MakePromotion(
            requestDto.Name,
            requestDto.DiscountPercentage,
            requestDto.StartDate,
            requestDto.EndDate
        );
    }

    public static PromotionResponseDto ToResponseDto(Promotion promotion, List<Product> products)
    {
        var productDtos = products
            .Select(p => new ProductSummaryDto(p.Id, p.Name))
            .ToList();

        return new PromotionResponseDto(
            promotion.Id,
            promotion.Name,
            promotion.DiscountPercentage,
            promotion.StartDate,
            promotion.EndDate,
            productDtos
        );
    }
}

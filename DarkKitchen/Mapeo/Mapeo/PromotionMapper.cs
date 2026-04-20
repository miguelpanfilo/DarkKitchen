using Contratos;
using Contratos.Promotions;
using Dominio;

namespace Mapeo;

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

    public static PromotionResponseDto ToResponseDto(Promotion promotion)
    {
        return new PromotionResponseDto(
            promotion.Id,
            promotion.Name,
            promotion.DiscountPercentage,
            promotion.StartDate,
            promotion.EndDate);
    }
}

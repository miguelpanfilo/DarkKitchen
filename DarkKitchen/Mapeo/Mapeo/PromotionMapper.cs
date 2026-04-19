using Contratos;
using Contratos.Promotions;
using Dominio;

namespace Mapeo;

public class PromotionMapper
{
    public static Promotion ToEntity(PromotionDto dto)
    {
        return Promotion.MakePromotion(
            dto.Name,
            dto.DiscountPercentage,
            dto.StartDate,
            dto.EndDate
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

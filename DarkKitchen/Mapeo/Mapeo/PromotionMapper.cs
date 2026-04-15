using Contratos;
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
}

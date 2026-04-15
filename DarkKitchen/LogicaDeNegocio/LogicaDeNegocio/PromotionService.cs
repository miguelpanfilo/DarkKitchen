using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;

namespace LogicaDeNegocio;

public class PromotionService
{
    private readonly IPromotionRepository _promotionRepository;

    public PromotionService(IPromotionRepository promotionRepository)
    {
        _promotionRepository = promotionRepository;
    }

    public Promotion Create(PromotionDto promotionDto)
    {
        var promotion = new Promotion(
            promotionDto.Name,
            promotionDto.DiscountPercentage,
            promotionDto.StartDate,
            promotionDto.EndDate
        );
        return _promotionRepository.Create(promotion);
    }
}

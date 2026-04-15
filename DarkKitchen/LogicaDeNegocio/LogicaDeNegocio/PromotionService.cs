using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Mapeo;

namespace LogicaDeNegocio;

public class PromotionService
{
    private readonly IPromotionRepository _promotionRepository;

    public PromotionService(IPromotionRepository promotionRepository)
    {
        _promotionRepository = promotionRepository;
    }

    public PromotionResponseDto Create(PromotionDto promotionDto)
    {
        var promotion = PromotionMapper.ToEntity(promotionDto);
        var createPromotion = _promotionRepository.Create(promotion);

        return PromotionMapper.ToResponseDto(createPromotion);
    }
}

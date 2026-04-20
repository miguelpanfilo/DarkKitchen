using Contratos.Promotions;

namespace LogicaDeNegocioAbstracciones.Promotions;

public interface IPromotionService
{
    PromotionResponseDto Create(PromotionRequestDto request);
}

using Contratos.Promotions;

namespace LogicaDeNegocioAbstracciones.Promotions;

public interface IPromotionService
{
    PromotionResponseDto Create(PromotionRequestDto request);
    PromotionResponseDto Update(int id, PromotionPatchDto request);
    PromotionResponseDto AssociateProduct(int promotionId, int productId);
    void Delete(int promotionId);
}

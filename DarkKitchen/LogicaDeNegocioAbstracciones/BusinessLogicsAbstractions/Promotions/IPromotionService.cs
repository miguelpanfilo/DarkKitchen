using Contract.Promotions;

namespace BusinessLogicsAbstractions.Promotions;

public interface IPromotionService
{
    PromotionResponseDto Create(PromotionRequestDto request);
    PromotionResponseDto Update(int id, PromotionPatchDto request,  string responsibleUser);
    PromotionResponseDto AssociateProduct(int promotionId, int productId);
    PromotionResponseDto DisassociateProduct(int promotionId, int productId);
    void Delete(int promotionId,  string responsibleUser);
    List<PromotionResponseDto> GetPromotions(PromotionSearchRequestDto request);
    decimal GetHighestCurrentDiscountForProduct(int productId);
}

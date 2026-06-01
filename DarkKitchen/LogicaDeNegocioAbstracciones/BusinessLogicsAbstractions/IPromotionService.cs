using Contract.Promotions;

namespace BusinessLogicsAbstractions;

public interface IPromotionService
{
    PromotionResponseDto Create(PromotionRequestDto request);
    PromotionResponseDto Update(int id, PromotionPatchDto request);
    PromotionResponseDto AssociateProduct(int promotionId, int productId);
    PromotionResponseDto DisassociateProduct(int promotionId, int productId);
    void Delete(int promotionId);
    List<PromotionResponseDto> GetPromotions(PromotionSearchRequestDto request);
    decimal GetHighestCurrentDiscountForProduct(int productId);
}

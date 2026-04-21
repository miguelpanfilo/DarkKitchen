using AlmacenamientoAbstracciones;
using Contratos.Promotions;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocioAbstracciones.Promotions;
using Mapeo;

namespace LogicaDeNegocio.Promotions;

public class PromotionService : IPromotionService
{
    private readonly IPromotionRepository _promotionRepository;
    private readonly IProductRepository _productRepository;

    public PromotionService(IPromotionRepository promotionRepository, IProductRepository productRepository)
    {
        _promotionRepository = promotionRepository;
        _productRepository = productRepository;
    }

    public PromotionResponseDto Create(PromotionRequestDto promotionRequestDto)
    {
        var promotion = PromotionMapper.ToEntity(promotionRequestDto);
        var createdPromotion = _promotionRepository.Create(promotion);

        return PromotionMapper.ToResponseDto(createdPromotion);
    }

    public void Delete(int promotionId)
    {
        var promotion = GetExistingPromotion(promotionId);

        promotion.Delete();

        _promotionRepository.Update(promotion);
    }

    public PromotionResponseDto Update(int promotionId, PromotionPatchDto promotionPatchDto)
    {
        var promotion = GetExistingPromotion(promotionId);

        UpdateNameIfProvided(promotion, promotionPatchDto.Name);
        UpdateDiscountIfProvided(promotion, promotionPatchDto.DiscountPercentage);
        UpdateDatesIfProvided(promotion, promotionPatchDto.StartDate, promotionPatchDto.EndDate);

        var updatedPromotion = _promotionRepository.Update(promotion);

        return PromotionMapper.ToResponseDto(updatedPromotion);
    }

    public PromotionResponseDto AssociateProduct(int promotionId, int productId)
    {
        var promotion = GetExistingPromotion(promotionId);

        GetExistingProduct(productId);

        promotion.AssociateProduct(productId);

        var updatedPromotion = _promotionRepository.Update(promotion);

        return PromotionMapper.ToResponseDto(updatedPromotion);
    }

    public PromotionResponseDto DisassociateProduct(int promotionId, int productId)
    {
        var promotion = GetExistingPromotion(promotionId);

        GetExistingProduct(productId);

        promotion.DisassociateProduct(productId);

        var updatedPromotion = _promotionRepository.Update(promotion);

        return PromotionMapper.ToResponseDto(updatedPromotion);
    }

    private Promotion GetExistingPromotion(int promotionId)
    {
        var promotion = _promotionRepository.GetById(promotionId);

        ValidatePromotionExists(promotion);

        return promotion;
    }

    private void GetExistingProduct(int productId)
    {
        var product = _productRepository.GetById(productId);
        ValidateProductExists(product);
    }

    private static void UpdateNameIfProvided(Promotion promotion, string? name)
    {
        if (name != null)
        {
            promotion.UpdateName(name);
        }
    }

    private static void UpdateDiscountIfProvided(Promotion promotion, decimal? discountPercentage)
    {
        if (discountPercentage != null)
        {
            promotion.UpdateDiscountPercentage(discountPercentage.Value);
        }
    }

    private static void UpdateDatesIfProvided(Promotion promotion, DateOnly? startDate, DateOnly? endDate)
    {
        if (startDate != null || endDate != null)
        {
            var newStartDate = startDate ?? promotion.StartDate;
            var newEndDate = endDate ?? promotion.EndDate;

            promotion.UpdateDates(newStartDate, newEndDate);
        }
    }

    private static void ValidatePromotionExists(Promotion? promotion)
    {
        if (promotion == null || promotion.IsDeleted)
        {
            throw new PromotionException("La promoción no existe");
        }
    }

    private static void ValidateProductExists(Product? product)
    {
        if (product == null || product.IsDeleted)
        {
            throw new PromotionException("El producto no existe");
        }
    }
}

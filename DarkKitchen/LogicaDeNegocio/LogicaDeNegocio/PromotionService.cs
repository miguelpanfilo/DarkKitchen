using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Exceptions;
using Mapeo;

namespace LogicaDeNegocio;

public class PromotionService
{
    private readonly IPromotionRepository _promotionRepository;
    private readonly IProductRepository _productRepository;

    public PromotionService(IPromotionRepository promotionRepository, IProductRepository productRepository)
    {
        _promotionRepository = promotionRepository;
        _productRepository = productRepository;
    }

    public PromotionResponseDto Create(PromotionDto promotionDto)
    {
        var promotion = PromotionMapper.ToEntity(promotionDto);
        var createdPromotion = _promotionRepository.Create(promotion);

        return PromotionMapper.ToResponseDto(createdPromotion);
    }

    public void Delete(int promotionId)
    {
        var promotion = GetExistingPromotion(promotionId);

        promotion.Deleted();

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

        var product = GetExistingProduct(productId);

        promotion.AssociateProduct(product);

        var updatedPromotion = _promotionRepository.Update(promotion);

        return PromotionMapper.ToResponseDto(updatedPromotion);
    }

    public PromotionResponseDto DisassociateProduct(int promotionId, int productId)
    {
        var promotion = GetExistingPromotion(promotionId);

        var product = GetExistingProduct(productId);

        promotion.DisassociateProduct(product);

        var updatedPromotion = _promotionRepository.Update(promotion);

        return PromotionMapper.ToResponseDto(updatedPromotion);
    }

    private Promotion GetExistingPromotion(int promotionId)
    {
        var promotion = _promotionRepository.GetById(promotionId);
        ValidatePromotionExists(promotion);
        return promotion;
    }

    private Product GetExistingProduct(int productId)
    {
        var product = _productRepository.GetById(productId);
        ValidateProductExists(product);
        return product;
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
        if (promotion == null)
        {
            throw new PromotionException("La promoción no existe");
        }
    }

    private static void ValidateProductExists(Product? product)
    {
        if (product == null)
        {
            throw new PromotionException("El producto no existe");
        }
    }
}

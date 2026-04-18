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
        var createPromotion = _promotionRepository.Create(promotion);

        return PromotionMapper.ToResponseDto(createPromotion);
    }

    public PromotionResponseDto Update(int promotionId, PromotionPatchDto promotionPatchDto)
    {
        var promotion = _promotionRepository.GetById(promotionId);

        if (promotionPatchDto.Name != null)
        {
            promotion.UpdateName(promotionPatchDto.Name);
        }

        if(promotionPatchDto.DiscountPercentage != null)
        {
            promotion.UpdateDiscountPercentage(promotionPatchDto.DiscountPercentage.Value);
        }

        if(promotionPatchDto.StartDate != null)
        {
            promotion.UpdateStartDate(promotionPatchDto.StartDate.Value);
        }

        var updatePromotion = _promotionRepository.Update(promotion);

        return PromotionMapper.ToResponseDto(updatePromotion);
    }

    public PromotionResponseDto AssociateProduct(int promotionId, int productId)
    {
        var promotion = _promotionRepository.GetById(promotionId);
        ValidatePromotionExists(promotion);

        var product = _productRepository.GetById(productId);
        ValidateProductExists(product);

        promotion.AssociateProduct(product);

        var updatePromotion = _promotionRepository.Update(promotion);

        return PromotionMapper.ToResponseDto(updatePromotion);
    }

    public PromotionResponseDto DisassociateProduct(int promotionId, int productId)
    {
        var promotion = _promotionRepository.GetById(promotionId);
        ValidatePromotionExists(promotion);

        var product = _productRepository.GetById(productId);
        ValidateProductExists(product);

        promotion.DisassociateProduct(product);

        var updatedPromotion = _promotionRepository.Update(promotion);

        return PromotionMapper.ToResponseDto(updatedPromotion);
    }

    private void ValidatePromotionExists(Promotion? promotion)
    {
        if (promotion == null)
        {
            throw new PromotionException("La promoción no existe");
        }
    }

    private void ValidateProductExists(Product? product)
    {
        if (product == null)
        {
            throw new PromotionException("El producto no existe");
        }
    }
}

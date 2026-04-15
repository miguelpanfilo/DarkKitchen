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

    public PromotionResponseDto AssociateProduct(int promotionId, int productId)
    {
        var promotion = _promotionRepository.GetById(promotionId);
        if (promotion == null)
        {
            throw new PromotionException("La promoción no existe");
        }

        var product = _productRepository.GetById(productId);
        if(product == null)
        {
            throw new PromotionException("El producto no existe");
        }

        promotion.AssociateProduct(product);

        var updatePromotion = _promotionRepository.Update(promotion);

        return PromotionMapper.ToResponseDto(updatePromotion);
    }
}

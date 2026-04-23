using System.ComponentModel.DataAnnotations;
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

        return MapPromotionWithProducts(createdPromotion);
    }

    public void Delete(int promotionId)
    {
        var promotion = GetExistingPromotion(promotionId);

        promotion.Delete();

        _promotionRepository.Update(promotion);
    }

    public List<PromotionResponseDto> GetPromotions(PromotionSearchRequestDto request)
    {
        ValidateDateIsProvided(request);

        var promotions = _promotionRepository.GetAll();

        var filtered = FilterByDate(promotions, request.Date.Value);
        filtered = FilterByAssociatedProductFilters(filtered, request.ProductId, request.CommercialLine);

        return filtered
            .Select(MapPromotionWithProducts)
            .ToList();
    }

    public PromotionResponseDto Update(int promotionId, PromotionPatchDto promotionPatchDto)
    {
        var promotion = GetExistingPromotion(promotionId);

        UpdateNameIfProvided(promotion, promotionPatchDto.Name);
        UpdateDiscountIfProvided(promotion, promotionPatchDto.DiscountPercentage);
        UpdateDatesIfProvided(promotion, promotionPatchDto.StartDate, promotionPatchDto.EndDate);

        var updatedPromotion = _promotionRepository.Update(promotion);

        return MapPromotionWithProducts(updatedPromotion);
    }

    public PromotionResponseDto AssociateProduct(int promotionId, int productId)
    {
        var promotion = GetExistingPromotion(promotionId);

        GetExistingProduct(productId);
        promotion.AssociateProduct(productId);

        var updatedPromotion = _promotionRepository.Update(promotion);

        return MapPromotionWithProducts(updatedPromotion);
    }

    public PromotionResponseDto DisassociateProduct(int promotionId, int productId)
    {
        var promotion = GetExistingPromotion(promotionId);

        GetExistingProduct(productId);

        promotion.DisassociateProduct(productId);

        var updatedPromotion = _promotionRepository.Update(promotion);

        return MapPromotionWithProducts(updatedPromotion);
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
        return product!;
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

    private PromotionResponseDto MapPromotionWithProducts(Promotion promotion)
    {
        var products = promotion.PromotionProducts
            .Select(pp => GetExistingProduct(pp.ProductId))
            .ToList();

        return PromotionMapper.ToResponseDto(promotion, products);
    }

    private static void ValidateDateIsProvided(PromotionSearchRequestDto request)
    {
        if (request.Date is null)
        {
            throw new ValidationException("La fecha es obligatoria");
        }
    }

    private static List<Promotion> FilterByDate(List<Promotion> promotions, DateOnly date)
    {
        return promotions
            .Where(p => p.StartDate <= date && p.EndDate >= date)
            .ToList();
    }

    private static List<Promotion> FilterByProductId(List<Promotion> promotions, int? productId)
    {
        if (productId is null)
        {
            return promotions;
        }

        return promotions
            .Where(p => p.PromotionProducts.Any(pp => pp.ProductId == productId.Value))
            .ToList();
    }

    private List<Promotion> FilterByCommercialLine(List<Promotion> promotions, string? commercialLine)
    {
        if (string.IsNullOrWhiteSpace(commercialLine))
        {
            return promotions;
        }

        return promotions
            .Where(p => p.PromotionProducts.Any(pp =>
                GetExistingProduct(pp.ProductId).CommercialLine == commercialLine))
            .ToList();
    }

    private List<Promotion> FilterByAssociatedProductFilters(
        List<Promotion> promotions,
        int? productId,
        string? commercialLine)
    {
        if (productId is null && string.IsNullOrWhiteSpace(commercialLine))
        {
            return promotions;
        }

        return promotions
            .Where(p => p.PromotionProducts.Any(pp =>
            {
                var product = GetExistingProduct(pp.ProductId);

                var matchesProductId = productId is null || product.Id == productId.Value;
                var matchesCommercialLine = string.IsNullOrWhiteSpace(commercialLine) ||
                                            product.CommercialLine == commercialLine;

                return matchesProductId && matchesCommercialLine;
            }))
            .ToList();
    }
}

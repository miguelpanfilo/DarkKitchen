using BusinessLogics.Shared;
using BusinessLogicsAbstractions;
using BusinessLogicsAbstractions.Exceptions;
using BusinessLogicsAbstractions.Promotions;
using Contract.Promotions;
using DataAccessAbstractions;
using Domain;
using Domain.Exceptions;
using Mappers;

namespace BusinessLogics.Promotions;

public class PromotionService : IPromotionService
{
    private readonly IPromotionRepository _promotionRepository;
    private readonly IProductRepository _productRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUserService _currentUserService;

    public PromotionService(IPromotionRepository promotionRepository,
        IProductRepository productRepository,
        IAuditLogService auditLogService,
        ICurrentUserService currentUserService)
    {
        _promotionRepository = promotionRepository;
        _productRepository = productRepository;
        _auditLogService = auditLogService;
        _currentUserService = currentUserService;
    }

    public PromotionResponseDto Create(PromotionRequestDto promotionRequestDto)
    {
        var promotion = ServiceExceptionHandler.ExecuteWithValidationHandling<Promotion, PromotionException>(() =>
            PromotionMapper.ToEntity(promotionRequestDto)
        );

        var createdPromotion = _promotionRepository.Create(promotion);
        _auditLogService.Register(AuditedEntities.Promotion, createdPromotion.Id, AuditDescriptions.PromotionCreated, _currentUserService.GetEmail());
        return MapPromotionWithProducts(createdPromotion);
    }

    public decimal GetHighestCurrentDiscountForProduct(int productId)
    {
        var promotions = _promotionRepository.GetAll();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var applicablePromotions = promotions
            .Where(p => !p.IsDeleted)
            .Where(p => p.StartDate <= today && p.EndDate >= today)
            .Where(p => p.PromotionProducts.Any(pp => pp.ProductId == productId))
            .ToList();

        if (!applicablePromotions.Any())
        {
            return 0m;
        }

        return applicablePromotions.Max(p => p.DiscountPercentage) / 100m;
    }

    public void Delete(int promotionId,  string responsibleUser)
    {
        var promotion = GetExistingPromotion(promotionId);

        promotion.Delete();

        _promotionRepository.Update(promotion);
        _auditLogService.Register(AuditedEntities.Promotion, promotion.Id,
            AuditDescriptions.PromotionDeleted, responsibleUser);
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
        _auditLogService.Register(AuditedEntities.Promotion, updatedPromotion.Id, AuditDescriptions.PromotionModified,
            _currentUserService.GetEmail());

        return MapPromotionWithProducts(updatedPromotion);
    }

    public PromotionResponseDto AssociateProduct(int promotionId, int productId)
    {
        var promotion = GetExistingPromotion(promotionId);

        GetExistingProduct(productId);
        ServiceExceptionHandler.ExecuteWithConflictHandling<PromotionException>(()
            => promotion.AssociateProduct(productId));

        var updatedPromotion = _promotionRepository.Update(promotion);

        return MapPromotionWithProducts(updatedPromotion);
    }

    public PromotionResponseDto DisassociateProduct(int promotionId, int productId)
    {
        var promotion = GetExistingPromotion(promotionId);

        GetExistingProduct(productId);

        ServiceExceptionHandler.ExecuteWithConflictHandling<PromotionException>(()
            => promotion.DisassociateProduct(productId));

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
            ServiceExceptionHandler.ExecuteWithValidationHandling<PromotionException>(()
                => promotion.UpdateName(name));
        }
    }

    private static void UpdateDiscountIfProvided(Promotion promotion, decimal? discountPercentage)
    {
        if (discountPercentage != null)
        {
            ServiceExceptionHandler.ExecuteWithValidationHandling<PromotionException>(() => promotion.UpdateDiscountPercentage(discountPercentage.Value));
        }
    }

    private static void UpdateDatesIfProvided(Promotion promotion, DateOnly? startDate, DateOnly? endDate)
    {
        if (startDate != null || endDate != null)
        {
            var newStartDate = startDate ?? promotion.StartDate;
            var newEndDate = endDate ?? promotion.EndDate;

            ServiceExceptionHandler.ExecuteWithValidationHandling<PromotionException>(()
                => promotion.UpdateDates(newStartDate, newEndDate));
        }
    }

    private static void ValidatePromotionExists(Promotion? promotion)
    {
        if (promotion == null || promotion.IsDeleted)
        {
            throw new NotFoundException("La promoción no existe");
        }
    }

    private static void ValidateProductExists(Product? product)
    {
        if (product == null || product.IsDeleted)
        {
            throw new NotFoundException("El producto no existe");
        }
    }

    private PromotionResponseDto MapPromotionWithProducts(Promotion promotion)
    {
        var products = promotion.PromotionProducts?
            .Select(pp => _productRepository.GetById(pp.ProductId))
            .Where(product => product is not null && !product.IsDeleted)
            .Select(product => product!)
            .ToList() ?? [];

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

                var matchesProductId = productId is null || pp.ProductId == productId.Value;
                var matchesCommercialLine = string.IsNullOrWhiteSpace(commercialLine) ||
                                            product.CommercialLine == commercialLine;

                return matchesProductId && matchesCommercialLine;
            }))
            .ToList();
    }
}

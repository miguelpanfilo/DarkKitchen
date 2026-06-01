using BusinessLogicsAbstractions;
using Contract.Importers;
using Contract.Products;
using DataAccessAbstractions;
using Domain;
using Domain.Exceptions;
using Mappers;

namespace BusinessLogics;

public class ProductService : IProductService, IProductCreatorService
{
    private readonly IProductRepository _repository;
    private readonly ProductPhotoService _productPhotoService;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUserService _currentUserService;

    public ProductService(IProductRepository repository,
        ProductPhotoService productPhotoService,
        IAuditLogService auditLogService,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _productPhotoService = productPhotoService;
        _auditLogService = auditLogService;
        _currentUserService = currentUserService;
    }

    public List<ProductResponseDto> GetProducts(ProductSearchRequestDto request)
    {
        var products = _repository.GetAll()
            .Where(p => !p.IsDeleted).ToList();

        products = FilterByName(products, request.Name);
        products = FilterByCategory(products, request.Category);
        products = FilterByCommercialLine(products, request.CommercialLine);

        return MapProducts(products);
    }

    private static List<ProductResponseDto> MapProducts(List<Product> products)
    {
        return products
            .Select(ProductMapper.ToResponseDto)
            .ToList();
    }

    private static List<Product> FilterByName(List<Product> products, string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return products;
        }

        var normalized = name.Trim();

        return products
            .Where(p => p.Name.Contains(normalized, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private static List<Product> FilterByCategory(List<Product> products, string? category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return products;
        }

        var normalized = category.Trim();

        return products
            .Where(p => p.Category.Contains(normalized, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private static List<Product> FilterByCommercialLine(List<Product> products, string? commercialLine)
    {
        if (string.IsNullOrWhiteSpace(commercialLine))
        {
            return products;
        }

        var normalized = commercialLine.Trim();

        return products
            .Where(p => p.CommercialLine.Contains(normalized, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public ProductResponseDto GetById(int productId)
    {
        var product = GetExistingProduct(productId);
        return ProductMapper.ToResponseDto(product);
    }

    public ProductResponseDto Create(ProductDTO productDto)
    {
        var savedPhotos = ServiceExceptionHandler.ExecuteWithValidationHandling<List<ProductPhoto>, ProductException>(() =>
            _productPhotoService.SavePhotos(productDto.Photos));
        var product = ServiceExceptionHandler.ExecuteWithValidationHandling<Product,
                ProductException>(() => ProductMapper.ToEntity(productDto, savedPhotos));

        var createdProduct = _repository.Create(product);
        _auditLogService.Register(AuditedEntities.Product, createdProduct.Id, AuditDescriptions.ProductCreated,
            _currentUserService.GetEmail());

        return ProductMapper.ToResponseDto(createdProduct);
    }

    public ProductResponseDto Update(int productId, ProductPatchDto patchDto)
    {
        var product = GetExistingProduct(productId);

        ApplyPatch(product, patchDto);
        var updatedProduct = _repository.Update(product);
        _auditLogService.Register(AuditedEntities.Product, updatedProduct.Id, AuditDescriptions.ProductModified, _currentUserService.GetEmail());

        return ProductMapper.ToResponseDto(updatedProduct);
    }

    public void Delete(int productId)
    {
        var product = GetExistingProduct(productId);

        ServiceExceptionHandler.ExecuteWithConflictHandling<ProductException>(() => product.Delete());
        _repository.Update(product);
        _auditLogService.Register(AuditedEntities.Product, product.Id, AuditDescriptions.ProductDeleted, _currentUserService.GetEmail());
    }

    private void ApplyPatch(Product product, ProductPatchDto patchDto)
    {
        UpdateCode(product, patchDto);
        UpdateName(product, patchDto);
        UpdateDescription(product, patchDto);
        UpdateCommercialLine(product, patchDto);
        UpdateCategory(product, patchDto);
        UpdatePrice(product, patchDto);
        UpdatePhotos(product, patchDto);
        UpdateActiveState(product, patchDto);
    }

    private static void UpdateCode(Product product, ProductPatchDto patchDto)
    {
        if (patchDto.Code != null)
        {
            ServiceExceptionHandler.ExecuteWithValidationHandling<ProductException>(()
                => product.UpdateCode(patchDto.Code));
        }
    }

    private static void UpdateName(Product product, ProductPatchDto patchDto)
    {
        if (patchDto.Name != null)
        {
            ServiceExceptionHandler.ExecuteWithValidationHandling<ProductException>(() => product.UpdateName(patchDto.Name));
        }
    }

    private static void UpdateDescription(Product product, ProductPatchDto patchDto)
    {
        if (patchDto.Description != null)
        {
            ServiceExceptionHandler.ExecuteWithValidationHandling<ProductException>(()
                => product.UpdateDescription(patchDto.Description));
        }
    }

    private static void UpdateCommercialLine(Product product, ProductPatchDto patchDto)
    {
        if (patchDto.CommercialLine != null)
        {
            ServiceExceptionHandler.ExecuteWithValidationHandling<ProductException>(()
                => product.UpdateCommercialLine(patchDto.CommercialLine));
        }
    }

    private static void UpdateCategory(Product product, ProductPatchDto patchDto)
    {
        if (patchDto.Category != null)
        {
            ServiceExceptionHandler.ExecuteWithValidationHandling<ProductException>(()
                => product.UpdateCategory(patchDto.Category));
        }
    }

    private static void UpdatePrice(Product product, ProductPatchDto patchDto)
    {
        if (patchDto.Price.HasValue)
        {
            ServiceExceptionHandler.ExecuteWithValidationHandling<ProductException>(()
                => product.UpdatePrice(patchDto.Price.Value));
        }
    }

    private void UpdatePhotos(Product product, ProductPatchDto patchDto)
    {
        if (patchDto.Photos != null)
        {
            var finalPhotos = ServiceExceptionHandler.ExecuteWithValidationHandling<List<ProductPhoto>,
                ProductException>(() => _productPhotoService.SyncPhotos(product, patchDto.Photos));
            ServiceExceptionHandler.ExecuteWithValidationHandling<ProductException>(()
                => product.UpdatePhotos(finalPhotos));
        }
    }

    private static void UpdateActiveState(Product product,  ProductPatchDto patchDto)
    {
        if (!patchDto.IsActive.HasValue)
        {
            return;
        }

        if (patchDto.IsActive.Value)
        {
            product.Activate();
        }
        else
        {
            product.Deactivate();
        }
    }

    private Product GetExistingProduct(int productId)
    {
        var product = _repository.GetById(productId);

        if (product == null || product.IsDeleted)
        {
            throw new NotFoundException("El producto no existe");
        }

        return product;
    }
}

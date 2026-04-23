using AlmacenamientoAbstracciones;
using Contratos.Products;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocioAbstracciones.Exceptions;
using LogicaDeNegocioAbstracciones.Products;
using Mapeo;

namespace LogicaDeNegocio.Products;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ProductPhotoService _productPhotoService;

    public ProductService(IProductRepository repository, ProductPhotoService productPhotoService)
    {
        _repository = repository;
        _productPhotoService = productPhotoService;
    }

    public List<ProductResponseDto> GetProducts(ProductSearchRequestDto request)
    {
        var products = _repository.GetAll();

        products = FilterByName(products, request.Name);
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

        return products
            .Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private static List<Product> FilterByCommercialLine(List<Product> products, string? commercialLine)
    {
        if (string.IsNullOrWhiteSpace(commercialLine))
        {
            return products;
        }

        return products
            .Where(p => p.CommercialLine.Equals(commercialLine, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public ProductResponseDto GetById(int productId)
    {
        var product = GetExistingProduct(productId);
        return ProductMapper.ToResponseDto(product);
    }

    public ProductResponseDto Create(ProductDTO productDto)
    {
        var savedPhotos = _productPhotoService.SavePhotos(productDto.Photos);

        var product = ExecuteWithValidationHandling(() => ProductMapper.ToEntity(productDto, savedPhotos));

        var createdProduct = _repository.Create(product);

        return ProductMapper.ToResponseDto(createdProduct);
    }

    public ProductResponseDto Update(int productId, ProductPatchDto patchDto)
    {
        var product = GetExistingProduct(productId);

        ApplyPatch(product, patchDto);
        var updatedProduct = _repository.Update(product);

        return ProductMapper.ToResponseDto(updatedProduct);
    }

    public void Delete(int productId)
    {
        var product = GetExistingProduct(productId);

        ExecuteWithConflictHandling(() => product.Delete());
        _repository.Update(product);
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
            ExecuteWithValidationHandling(() => product.UpdateCode(patchDto.Code));
        }
    }

    private static void UpdateName(Product product, ProductPatchDto patchDto)
    {
        if (patchDto.Name != null)
        {
            ExecuteWithValidationHandling(() => product.UpdateName(patchDto.Name));
        }
    }

    private static void UpdateDescription(Product product, ProductPatchDto patchDto)
    {
        if (patchDto.Description != null)
        {
            ExecuteWithValidationHandling(() => product.UpdateDescription(patchDto.Description));
        }
    }

    private static void UpdateCommercialLine(Product product, ProductPatchDto patchDto)
    {
        if (patchDto.CommercialLine != null)
        {
            ExecuteWithValidationHandling(() => product.UpdateCommercialLine(patchDto.CommercialLine));
        }
    }

    private static void UpdateCategory(Product product, ProductPatchDto patchDto)
    {
        if (patchDto.Category != null)
        {
            ExecuteWithValidationHandling(() => product.UpdateCategory(patchDto.Category));
        }
    }

    private static void UpdatePrice(Product product, ProductPatchDto patchDto)
    {
        if (patchDto.Price.HasValue)
        {
            ExecuteWithValidationHandling(() => product.UpdatePrice(patchDto.Price.Value));
        }
    }

    private void UpdatePhotos(Product product, ProductPatchDto patchDto)
    {
        if (patchDto.Photos != null)
        {
            var finalPhotos = ExecuteWithValidationHandling(() => _productPhotoService.SyncPhotos(product, patchDto.Photos));
            ExecuteWithValidationHandling(() => product.UpdatePhotos(finalPhotos));
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

    private static void ExecuteWithValidationHandling(Action action)
    {
        try
        {
            action();
        }
        catch (ProductException ex)
        {
            throw new ValidationException(ex.Message);
        }
    }

    private static void ExecuteWithConflictHandling(Action action)
    {
        try
        {
            action();
        }
        catch (ProductException ex)
        {
            throw new ConflictException(ex.Message);
        }
    }

    private static T ExecuteWithValidationHandling<T>(Func<T> action)
    {
        try
        {
            return action();
        }
        catch (ProductException ex)
        {
            throw new ValidationException(ex.Message);
        }
    }
}

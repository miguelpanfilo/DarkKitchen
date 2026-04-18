using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Exceptions;
using LogicaDeNegocioAbstracciones;
using Mapeo;

namespace LogicaDeNegocio;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ProductPhotoService _productPhotoService;

    public ProductService(IProductRepository repository, ProductPhotoService productPhotoService)
    {
        _repository = repository;
        _productPhotoService = productPhotoService;
    }

    public List<ProductResponseDto> GetAll()
    {
        var products = _repository.GetAll();

        return products
            .Select(ProductMapper.ToResponseDto)
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
        var product = ProductMapper.ToEntity(productDto, savedPhotos);
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

        _productPhotoService.DeletePhotos(product.Photos);
        _repository.Delete(productId);
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
            product.UpdateCode(patchDto.Code);
        }
    }

    private static void UpdateName(Product product, ProductPatchDto patchDto)
    {
        if (patchDto.Name != null)
        {
            product.UpdateName(patchDto.Name);
        }
    }

    private static void UpdateDescription(Product product, ProductPatchDto patchDto)
    {
        if (patchDto.Description != null)
        {
            product.UpdateDescription(patchDto.Description);
        }
    }

    private static void UpdateCommercialLine(Product product, ProductPatchDto patchDto)
    {
        if (patchDto.CommercialLine != null)
        {
            product.UpdateCommercialLine(patchDto.CommercialLine);
        }
    }

    private static void UpdateCategory(Product product, ProductPatchDto patchDto)
    {
        if (patchDto.Category != null)
        {
            product.UpdateCategory(patchDto.Category);
        }
    }

    private static void UpdatePrice(Product product, ProductPatchDto patchDto)
    {
        if (patchDto.Price.HasValue)
        {
            product.UpdatePrice(patchDto.Price.Value);
        }
    }

    private void UpdatePhotos(Product product, ProductPatchDto patchDto)
    {
        if (patchDto.Photos != null)
        {
            var finalPhotos = _productPhotoService.SyncPhotos(product, patchDto.Photos);
            product.UpdatePhotos(finalPhotos);
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

        if (product == null)
        {
            throw new ProductException("El producto no existe");
        }

        return product;
    }
}

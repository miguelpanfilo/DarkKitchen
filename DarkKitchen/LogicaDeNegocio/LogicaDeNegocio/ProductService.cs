using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Exceptions;
using Mapeo;

namespace LogicaDeNegocio;

public class ProductService
{
    private readonly IProductRepository _repository;
    private readonly ProductPhotoService _productPhotoService;

    public ProductService(IProductRepository repository, ProductPhotoService productPhotoService)
    {
        _repository = repository;
        _productPhotoService = productPhotoService;
    }

    public Product Create(ProductDTO productDto)
    {
        var savedPhotos = _productPhotoService.SavePhotos(productDto.Photos);
        var product = ProductMapper.ToEntity(productDto, savedPhotos);

        return _repository.Create(product);
    }

    public Product Update(int productId, ProductPatchDto patchDto)
    {
        var product = _repository.FindById(productId);

        if (product == null)
        {
            throw new ProductException("El producto no existe");
        }

        ApplyPatch(product, patchDto);

        return _repository.Update(product);
    }

    public void Delete(int productId)
    {
        var product = _repository.FindById(productId);

        _productPhotoService.DeletePhotos(product.Photos);
        _repository.Delete(productId);
    }

    private void ApplyPatch(Product product, ProductPatchDto patchDto)
    {
        if (patchDto.Code != null)
        {
            product.UpdateCode(patchDto.Code);
        }

        if (patchDto.Name != null)
        {
            product.UpdateName(patchDto.Name);
        }

        if (patchDto.Description != null)
        {
            product.UpdateDescription(patchDto.Description);
        }

        if (patchDto.CommercialLine != null)
        {
            product.UpdateCommercialLine(patchDto.CommercialLine);
        }

        if (patchDto.Category != null)
        {
            product.UpdateCategory(patchDto.Category);
        }

        if (patchDto.Price.HasValue)
        {
            product.UpdatePrice(patchDto.Price.Value);
        }

        if (patchDto.Photos != null)
        {
            var finalPhotos = _productPhotoService.SyncPhotos(product, patchDto.Photos);
            product.UpdatePhotos(finalPhotos);
        }

        if (patchDto.IsActive.HasValue)
        {
            if (patchDto.IsActive.Value)
            {
                product.Activate();
            }
            else
            {
                product.Deactivate();
            }
        }
    }
}

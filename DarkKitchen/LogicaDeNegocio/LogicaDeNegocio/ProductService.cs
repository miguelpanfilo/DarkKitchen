using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Mapeo;

namespace LogicaDeNegocio;

public class ProductService
{
    private readonly IProductRepository _repository;
    private readonly IProductPhotoStorage _productPhotoStorage;
    public ProductService(IProductRepository repository,IProductPhotoStorage photoStorage)
    {
        _repository = repository;
        _productPhotoStorage = photoStorage;
    }

    public Product Create(ProductDTO productDto)
    {
        SavePhotos(productDto);

        var product = ProductMapper.ToEntity(productDto);
        return _repository.Create(product);
    }

    private void SavePhotos(ProductDTO productDto)
    {
        foreach (var photo in productDto.Photos)
        {
            _productPhotoStorage.Save(photo);
        }
    }
}

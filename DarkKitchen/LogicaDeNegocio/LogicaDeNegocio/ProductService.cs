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
       var savePhotos = SavePhotos(productDto);

        var product = ProductMapper.ToEntity(productDto, savePhotos);
        return _repository.Create(product);
    }

    private List<string> SavePhotos(ProductDTO productDto)
    {
        var savedPhotos = new List<string>();

        foreach (var photo in productDto.Photos)
        {
            var path = _productPhotoStorage.Save(photo);
            savedPhotos.Add(path);
        }

        return savedPhotos;
    }
}

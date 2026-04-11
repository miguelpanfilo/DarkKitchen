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
       var savePhotos = SavePhotos(productDto.Photos);

        var product = ProductMapper.ToEntity(productDto, savePhotos);
        return _repository.Create(product);
    }

    private List<ProductPhoto> SavePhotos(List<PhotoDTO> photos)
    {
        var savedPhotos = new List<ProductPhoto>();

        foreach (var photoDto in photos)
        {
            var savedPath = _productPhotoStorage.Save(photoDto);
            savedPhotos.Add(new ProductPhoto(savedPath));
        }

        return savedPhotos;
    }
}

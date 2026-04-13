using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Exceptions;

namespace LogicaDeNegocio;

public class ProductPhotoService
{
    private const int MinPhotos = 1;
    private const int MaxPhotos = 3;

    private readonly IProductPhotoStorage _productPhotoStorage;

    public ProductPhotoService(IProductPhotoStorage productPhotoStorage)
    {
        _productPhotoStorage = productPhotoStorage;
    }

    public List<ProductPhoto> SavePhotos(List<PhotoDTO> photos)
    {
        ValidatePhotosCount(photos.Count);
        ValidateFormatPhotos(photos);

        var savedPhotos = new List<ProductPhoto>();

        foreach (var photo in photos)
        {
            savedPhotos.Add(SavePhoto(photo));
        }

        return savedPhotos;
    }

    public List<ProductPhoto> SyncPhotos(Product product, List<ProductPatchPhotoDto> requestedPhotos)
    {
        ValidatePhotosCount(requestedPhotos.Count);

        var finalPhotos = new List<ProductPhoto>();

        foreach (var requestedPhoto in requestedPhotos)
        {
            finalPhotos.Add(ResolvePhoto(product, requestedPhoto));
        }

        return finalPhotos;
    }

    private ProductPhoto ResolvePhoto(Product product, ProductPatchPhotoDto requestedPhoto)
    {
        if (!string.IsNullOrWhiteSpace(requestedPhoto.Path))
        {
            return GetExistingPhoto(product, requestedPhoto.Path);
        }

        ValidateNewPhoto(requestedPhoto);

        var photoDto = new PhotoDTO(
            requestedPhoto.FileName!,
            requestedPhoto.Extension!,
            requestedPhoto.SizeInBytes!.Value,
            requestedPhoto.Base64!
        );

        ValidateFormatPhoto(photoDto);

        return SavePhoto(photoDto);
    }

    private ProductPhoto GetExistingPhoto(Product product, string path)
    {
        var existingPhoto = product.Photos.FirstOrDefault(photo => photo.Path == path);

        if (existingPhoto == null)
        {
            throw new ProductException("La foto no pertenece al producto");
        }

        return existingPhoto;
    }

    private ProductPhoto SavePhoto(PhotoDTO photoDto)
    {
        var savedPath = _productPhotoStorage.Save(photoDto);
        return new ProductPhoto(savedPath);
    }

    private void ValidatePhotosCount(int count)
    {
        if (count < MinPhotos || count > MaxPhotos)
        {
            throw new ProductException("El producto debe tener entre 1 y 3 fotos");
        }
    }

    private void ValidateFormatPhotos(List<PhotoDTO> photos)
    {
        foreach (var photo in photos)
        {
            ValidateFormatPhoto(photo);
        }
    }

    private void ValidateFormatPhoto(PhotoDTO photoDto)
    {
        var ext = photoDto.Extension.Trim().ToLower().Replace(".", string.Empty);

        if (ext != "jpg")
        {
            throw new ProductException("Las fotos deben estar en formato JPG");
        }

        if (photoDto.SizeInBytes > 500 * 1024)
        {
            throw new ProductException("La foto no puede superar los 500 KB");
        }
    }

    private void ValidateNewPhoto(ProductPatchPhotoDto photo)
    {
        if (string.IsNullOrWhiteSpace(photo.FileName) ||
            string.IsNullOrWhiteSpace(photo.Extension) ||
            !photo.SizeInBytes.HasValue ||
            string.IsNullOrWhiteSpace(photo.Base64))
        {
            throw new ProductException("Datos de foto inválidos");
        }
    }
}

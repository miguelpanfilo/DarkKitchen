using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Exceptions;

namespace LogicaDeNegocio;

public class ProductPhotoService
{
    private const int MinPhotos = 1;
    private const int MaxPhotos = 3;
    private const long MaxPhotoSizeInBytes = 500 * 1024;

    private readonly IProductPhotoStorage _productPhotoStorage;

    public ProductPhotoService(IProductPhotoStorage productPhotoStorage)
    {
        _productPhotoStorage = productPhotoStorage;
    }

    public List<ProductPhoto> SavePhotos(List<PhotoDTO> photos)
    {
        ValidatePhotosCollection(photos);
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
        ValidatePhotosCollection(requestedPhotos);

        var finalPhotos = new List<ProductPhoto>();

        foreach (var requestedPhoto in requestedPhotos)
        {
            finalPhotos.Add(ResolvePhoto(product, requestedPhoto));
        }

        return finalPhotos;
    }

    public void DeletePhotos(List<ProductPhoto> photos)
    {
        foreach (var photo in photos)
        {
            _productPhotoStorage.Delete(photo.Path);
        }
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

    private void ValidatePhotosCollection<T>(ICollection<T>? photos)
    {
        if (photos == null || photos.Count < MinPhotos || photos.Count > MaxPhotos)
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
        if (string.IsNullOrWhiteSpace(photoDto.Base64))
        {
            throw new ProductException("La foto debe tener contenido en base64");
        }

        var ext = photoDto.Extension.Trim().ToLower().Replace(".", string.Empty);

        if (ext != "jpg")
        {
            throw new ProductException("Las fotos deben estar en formato JPG");
        }

        try
        {
            _ = Convert.FromBase64String(photoDto.Base64);
        }
        catch (FormatException)
        {
            throw new ProductException("La foto debe tener un base64 válido");
        }

        ValidatePhotoSizeFromBase64(photoDto.Base64);
    }

    private void ValidatePhotoSizeFromBase64(string base64)
    {
        try
        {
            var bytes = Convert.FromBase64String(base64);

            if (bytes.Length > MaxPhotoSizeInBytes)
            {
                throw new ProductException("La foto no puede superar los 500 KB");
            }
        }
        catch (FormatException)
        {
            throw new ProductException("La foto debe tener un base64 válido");
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

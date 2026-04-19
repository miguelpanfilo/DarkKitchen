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
        ValidatePhotos(photos);

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

        var removedPhotos = GetRemovedPhotos(product, finalPhotos);

        DeletePhotos(removedPhotos);

        return finalPhotos;
    }

    private List<ProductPhoto> GetRemovedPhotos(Product product, List<ProductPhoto> finalPhotos)
    {
        var finalPaths = finalPhotos.Select(photo => photo.Path).ToHashSet();

        return product.Photos
            .Where(photo => !finalPaths.Contains(photo.Path))
            .ToList();
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

        ValidatePhoto(photoDto);

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

    private void ValidatePhotos(List<PhotoDTO> photos)
    {
        foreach (var photo in photos)
        {
            ValidatePhoto(photo);
        }
    }

    private void ValidatePhoto(PhotoDTO photoDto)
    {
        ValidateBase64Content(photoDto.Base64);
        ValidateJpgExtension(photoDto.Extension);

        ValidatePhotoSize(photoDto.SizeInBytes);
    }

    private static void ValidateBase64Content(string base64)
    {
        if (string.IsNullOrWhiteSpace(base64))
        {
            throw new ProductException("La foto debe tener un base64 válido");
        }

        DecodeBase64(base64);
    }

    private static void ValidateJpgExtension(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
        {
            throw new ProductException("Las fotos deben estar en formato JPG");
        }

        var normalizedExtension = extension.Trim().ToLower().Replace(".", string.Empty);

        if (normalizedExtension != "jpg")
        {
            throw new ProductException("Las fotos deben estar en formato JPG");
        }
    }

    private static byte[] DecodeBase64(string base64)
    {
        try
        {
            return Convert.FromBase64String(base64);
        }
        catch (FormatException)
        {
            throw new ProductException("La foto debe tener un base64 válido");
        }
    }

    private void ValidatePhotoSize(int sizeInBytes)
    {
        if (sizeInBytes > MaxPhotoSizeInBytes)
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

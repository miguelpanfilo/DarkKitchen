using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Exceptions;
using Mapeo;

namespace LogicaDeNegocio;

public class ProductService
{
    private const int MinPhotos = 1;
    private const int MaxPhotos = 3;

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
            var finalPhotos = SyncPhotos(product, patchDto.Photos);
            product.UpdatePhotos(finalPhotos);
        }
    }

    private List<ProductPhoto> SavePhotos(List<PhotoDTO> photos)
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

    private void ValidateFormatPhotos(List<PhotoDTO> photoDtos)
    {
        foreach(var photoDto in photoDtos)
        {
            ValidateFormatPhoto(photoDto);
        }
    }

    private ProductPhoto SavePhoto(PhotoDTO photoDto)
    {
        var savedPath = _productPhotoStorage.Save(photoDto);
        return new ProductPhoto(savedPath);
    }

    private List<ProductPhoto> SyncPhotos(Product product, List<ProductPatchPhotoDto> requestedPhotos)
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

    private void ValidatePhotosCount(int count)
    {
        if (count < MinPhotos || count > MaxPhotos)
        {
            throw new ProductException("El producto debe tener entre 1 y 3 fotos");
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

    private void ValidateFormatPhoto(PhotoDTO photoDto)
    {
        var ext = photoDto.Extension.Trim().ToLower().Replace(".", string.Empty);

        if (ext != "jpg")
        {
            throw new ProductException("Las fotos deben estar en formato JPG");
        }
    }
}

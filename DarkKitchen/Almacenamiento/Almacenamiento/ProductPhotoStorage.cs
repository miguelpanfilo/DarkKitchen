using AlmacenamientoAbstracciones;
using Contratos;

namespace Almacenamiento;

public class ProductPhotoStorage : IProductPhotoStorage
{
    private readonly string _basePath;

    public ProductPhotoStorage(string basePath)
    {
        _basePath = basePath;
    }

    public string Save(PhotoDTO photo)
    {
        var bytes = Convert.FromBase64String(photo.Base64);
        var fileName = $"{Guid.NewGuid()}{photo.Extension}";
        var fullPath = Path.Combine(_basePath, fileName);

        File.WriteAllBytes(fullPath, bytes);

        return fullPath;
    }

    public void Delete(string path) => throw new NotImplementedException();
}

using AlmacenamientoAbstracciones;

namespace Almacenamiento;

public class ProductPhotoStorage : IProductPhotoStorage
{
    private readonly string _basePath;

    public ProductPhotoStorage(string basePath)
    {
        _basePath = basePath;
    }

    public string Save(string base64, string extension)
    {
        Directory.CreateDirectory(_basePath);

        var bytes = Convert.FromBase64String(base64);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(_basePath, fileName);

        File.WriteAllBytes(fullPath, bytes);

        return fullPath;
    }

    public void Delete(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}

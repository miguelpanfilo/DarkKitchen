using DataAccessAbstractions;

namespace DataAccess.Repositories;

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
        var normalizedExtension = extension.StartsWith('.') ? extension : $".{extension}";
        var fileName = $"{Guid.NewGuid()}{normalizedExtension}";
        var fullPath = Path.Combine(_basePath, fileName);

        File.WriteAllBytes(fullPath, bytes);

        return $"/photos/{fileName}";
    }

    public void Delete(string url)
    {
        var fullPath = Path.Combine(_basePath, Path.GetFileName(url));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}

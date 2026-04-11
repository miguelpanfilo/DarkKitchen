namespace Dominio;

public class ProductPhoto
{
    public string Path { get; set; } = string.Empty;

    public ProductPhoto()
    {
    }

    public ProductPhoto(string path)
    {
        Path = path;
    }
}

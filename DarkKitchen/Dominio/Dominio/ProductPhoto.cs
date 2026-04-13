using System.Diagnostics.CodeAnalysis;

namespace Dominio;

public class ProductPhoto
{
    public int Id { get; private set; }
    public string Path { get; set; } = string.Empty;

    [ExcludeFromCodeCoverage]
    public ProductPhoto()
    {
    }

    public ProductPhoto(string path)
    {
        Path = path;
    }
}

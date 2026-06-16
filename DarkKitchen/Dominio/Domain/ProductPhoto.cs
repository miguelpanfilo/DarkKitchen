using System.Diagnostics.CodeAnalysis;

namespace Domain;

public class ProductPhoto
{
    public int Id { get; private set; }
    public string Path { get; set; } = string.Empty;
    public int ProductId { get; private set; }

    [ExcludeFromCodeCoverage]
    public ProductPhoto()
    {
    }

    public ProductPhoto(string path)
    {
        Path = path;
    }
}

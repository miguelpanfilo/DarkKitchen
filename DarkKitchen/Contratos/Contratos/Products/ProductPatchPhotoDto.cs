namespace Contratos.Products;

public class ProductPatchPhotoDto
{
        public string? Path { get; init; }
        public string? FileName { get; init; }
        public string? Extension { get; init; }
        public int? SizeInBytes { get; init; }
        public string? Base64 { get; init; }
}

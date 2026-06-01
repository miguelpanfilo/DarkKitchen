namespace Contract;

public class ProductImportRequestDto
{
    public string ImporterName { get; set; } = string.Empty;

    public Stream FileStream { get; set; } = null!;

    public string FileName { get; set; } = string.Empty;
}

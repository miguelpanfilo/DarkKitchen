namespace Contract;

public class ProductResponseDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string CommercialLine { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public bool IsActive { get; init; }
    public List<PhotoResponseDto> Photos { get; init; } = new();
}

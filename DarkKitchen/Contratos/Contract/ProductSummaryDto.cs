namespace Contract;

public class ProductSummaryDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;

    public ProductSummaryDto(int id, string name)
    {
        Id = id;
        Name = name;
    }
}

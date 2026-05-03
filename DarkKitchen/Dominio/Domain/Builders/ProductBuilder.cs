namespace Domain.Builders;

public class ProductBuilder
{
    private string _code = string.Empty;
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _commercialLine = string.Empty;
    private string _category = string.Empty;
    private decimal _price;
    private List<ProductPhoto> _photos = new ();

    public ProductBuilder WithCode(string code)
    {
        _code = code;
        return this;
    }

    public ProductBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ProductBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public ProductBuilder WithCommercialLine(string commercialLine)
    {
        _commercialLine = commercialLine;
        return this;
    }

    public ProductBuilder WithCategory(string category)
    {
        _category = category;
        return this;
    }

    public ProductBuilder WithPrice(decimal price)
    {
        _price = price;
        return this;
    }

    public ProductBuilder WithPhotos(List<ProductPhoto> photos)
    {
        _photos = photos;
        return this;
    }

    public Product Build()
    {
        return Product.MakeProduct(
            _code,
            _name,
            _description,
            _commercialLine,
            _category,
            _price,
            _photos
        );
    }
}

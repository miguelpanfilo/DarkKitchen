namespace Domain;

public class Product
{
    public string Code { get; }
    public string Name { get; }
    public string Description { get; }
    public string CommercialLine { get; }
    public string Category { get; }

    public Product(string code, string name, string description, string commercialLine, string category)
    {
        Code = code;
        Name = name;
        Description = description;
        CommercialLine = commercialLine;
        Category = category;
    }
}

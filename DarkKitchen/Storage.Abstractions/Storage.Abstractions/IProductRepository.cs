using Domain;

namespace Storage.Abstractions;

public interface IProductRepository
{
    public Product Create(Product product);
}

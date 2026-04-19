using Dominio;

namespace AlmacenamientoAbstracciones;

public interface IProductRepository
{
    public Product Create(Product product);
    public Product GetById(int productId);
    public Product Update(Product product);
    public List<Product> GetAll();
}

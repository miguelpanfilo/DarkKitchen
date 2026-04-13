using Dominio;

namespace AlmacenamientoAbstracciones;

public interface IProductRepository
{
    public Product Create(Product product);
    public Product FindById(int productId);
    public Product Update(Product product);
    public List<Product> GetAll();
    public void Delete(int productId);
}

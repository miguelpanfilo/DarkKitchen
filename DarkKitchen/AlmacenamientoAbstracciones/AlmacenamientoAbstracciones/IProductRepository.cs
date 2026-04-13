using Dominio;

namespace AlmacenamientoAbstracciones;

public interface IProductRepository
{
    public Product Create(Product product);
    public Product FindById(int id);
    public Product Update(Product product);
}

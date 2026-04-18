using AlmacenamientoAbstracciones;
using Dominio;

namespace Almacenamiento;

public class ProductRepository : IProductRepository
{
    public Product Create(Product product) => throw new NotImplementedException();

    public Product FindById(int productId) => throw new NotImplementedException();

    public Product Update(Product product) => throw new NotImplementedException();

    public List<Product> GetAll() => throw new NotImplementedException();

    public void Delete(int productId) => throw new NotImplementedException();
}

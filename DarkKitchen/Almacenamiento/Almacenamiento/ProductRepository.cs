using AlmacenamientoAbstracciones;
using Dominio;

namespace Almacenamiento;

public class ProductRepository : IProductRepository
{
    private readonly SqlContext _context;

    public ProductRepository(SqlContext context)
    {
        _context = context;
    }

    public Product Create(Product product)
    {
        _context.Products.Add(product);
        _context.SaveChanges();
        return product;
    }

    public Product GetById(int productId) => throw new NotImplementedException();

    public Product Update(Product product) => throw new NotImplementedException();

    public List<Product> GetAll() => throw new NotImplementedException();
}

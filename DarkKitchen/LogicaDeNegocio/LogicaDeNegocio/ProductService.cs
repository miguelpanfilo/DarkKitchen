using AlmacenamientoAbstracciones;
using Dominio;

namespace LogicaDeNegocio;

public class ProductService
{
    private readonly IProductRepository _repository;
    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public Product Create(Product product)
    {
        return _repository.Create(product);
    }
}

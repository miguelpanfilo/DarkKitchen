using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Mapeo;

namespace LogicaDeNegocio;

public class ProductService
{
    private readonly IProductRepository _repository;
    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public Product Create(ProductDTO productDto)
    {
        var product = ProductMapper.ToEntity(productDto);
        return _repository.Create(product);
    }
}

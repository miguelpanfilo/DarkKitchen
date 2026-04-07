using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;

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
        var product = new Product(
            productDto.Code,
            productDto.Name,
            productDto.Description,
            productDto.CommercialLine,
            productDto.Category
        );
        return _repository.Create(product);
    }
}

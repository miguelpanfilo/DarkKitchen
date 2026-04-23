using Contratos.Products;

namespace LogicaDeNegocioAbstracciones.Products;

public interface IProductService
{
    ProductResponseDto GetById(int id);
    ProductResponseDto Create(ProductDTO request);
}

using Contratos.Products;

namespace LogicaDeNegocioAbstracciones.Products;

public interface IProductService
{
    ProductResponseDto GetById(int id);
    ProductResponseDto Create(ProductDTO request);
    List<ProductResponseDto> GetProducts(ProductSearchRequestDto request);
    ProductResponseDto Update(int productId, ProductPatchDto request);
    void Delete(int productId);
}

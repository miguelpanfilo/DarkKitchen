using Contract.Products;

namespace BusinessLogicsAbstractions.Products;

public interface IProductService
{
    ProductResponseDto GetById(int id);
    ProductResponseDto Create(ProductDTO request);
    List<ProductResponseDto> GetProducts(ProductSearchRequestDto request);
    ProductResponseDto Update(int productId, ProductPatchDto request, string responsibleUser);
    void Delete(int productId, string responsibleUser);
}

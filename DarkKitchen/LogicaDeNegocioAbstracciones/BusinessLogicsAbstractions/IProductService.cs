using Contract;

namespace BusinessLogicsAbstractions;

public interface IProductService
{
    ProductResponseDto GetById(int id);
    ProductResponseDto Create(ProductDTO request);
    List<ProductResponseDto> GetProducts(ProductSearchRequestDto request);
    ProductResponseDto Update(int productId, ProductPatchDto request);
    void Delete(int productId);
    Dictionary<int, string> GetNamesByIds(List<int> ids);
}

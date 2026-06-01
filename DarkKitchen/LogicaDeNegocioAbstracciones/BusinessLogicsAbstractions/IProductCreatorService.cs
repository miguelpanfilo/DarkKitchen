using Contract.Products;

namespace BusinessLogicsAbstractions;

public interface IProductCreatorService
{
    ProductResponseDto Create(ProductDTO productDto);
}

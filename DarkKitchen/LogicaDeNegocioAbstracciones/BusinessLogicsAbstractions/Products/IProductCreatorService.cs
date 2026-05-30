using Contract.Products;

namespace BusinessLogicsAbstractions.Products;

public interface IProductCreatorService
{
    ProductResponseDto Create(ProductDTO productDto);
}

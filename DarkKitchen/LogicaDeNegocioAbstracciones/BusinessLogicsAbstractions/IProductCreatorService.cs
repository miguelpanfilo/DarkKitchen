using Contract;

namespace BusinessLogicsAbstractions;

public interface IProductCreatorService
{
    ProductResponseDto Create(ProductDTO productDto);
}

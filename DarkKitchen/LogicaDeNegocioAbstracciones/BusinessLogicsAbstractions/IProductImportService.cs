using Contract;

namespace BusinessLogicsAbstractions;

public interface IProductImportService
{
    List<ProductResponseDto> ImportProducts(ProductImportRequestDto request);
}

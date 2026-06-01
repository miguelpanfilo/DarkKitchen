using Contract.Importers;
using Contract.Products;

namespace BusinessLogicsAbstractions;

public interface IProductImportService
{
    List<ProductResponseDto> ImportProducts(ProductImportRequestDto request);
}

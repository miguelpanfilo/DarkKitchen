using Contract.Importers;
using Contract.Products;

namespace BusinessLogicsAbstractions.Imports;

public interface IProductImportService
{
    List<ProductResponseDto> ImportProducts(ProductImportRequestDto request);
}

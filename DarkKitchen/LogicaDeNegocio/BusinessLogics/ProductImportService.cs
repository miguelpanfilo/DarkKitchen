using BusinessLogicsAbstractions.Imports;
using Contract.Importers;
using Contract.Products;

namespace BusinessLogics;

public class ProductImportService(IProductImporterProvider importerProvider) : IProductImportService
{
    public List<ProductResponseDto> ImportProducts(ProductImportRequestDto request)
    {
        throw new NotImplementedException();
    }
}

using BusinessLogicsAbstractions.Imports;
using Contract.Importers;
using Contract.Products;

namespace BusinessLogics;

public class ProductImportService(IProductImporterProvider importerProvider) : IProductImportService
{
    public List<ProductResponseDto> ImportProducts(ProductImportRequestDto request)
    {
        var importer = importerProvider
            .GetImporters()
            .First(i => i.Name == request.ImporterName);

        importer.Import(request.FileStream);

        return [];
    }
}

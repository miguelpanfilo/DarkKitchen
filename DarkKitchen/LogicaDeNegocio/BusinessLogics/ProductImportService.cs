using BusinessLogicsAbstractions;
using BusinessLogicsAbstractions.Imports;
using BusinessLogicsAbstractions.Products;
using Contract.Importers;
using Contract.Products;

namespace BusinessLogics;

public class ProductImportService(
    IProductImporterProvider importerProvider,
    IImportedProductCreatorService importedProductCreatorService) : IProductImportService
{
    public List<ProductResponseDto> ImportProducts(ProductImportRequestDto request)
    {
        var importer = importerProvider
            .GetImporters()
            .First(i => i.Name == request.ImporterName);

        var importedProducts = importer.Import(request.FileStream);

        return importedProducts
            .Select(importedProductCreatorService.CreateImportedProduct)
            .ToList();
    }
}

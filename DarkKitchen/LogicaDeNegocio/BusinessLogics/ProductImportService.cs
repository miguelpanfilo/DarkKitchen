using BusinessLogicsAbstractions;
using BusinessLogicsAbstractions.Exceptions;
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
        if(request == null)
        {
            throw new ValidationException(
                "La solicitud de importación es requerida");
        }

        var importer = importerProvider
            .GetImporters()
            .FirstOrDefault(i => i.Name == request.ImporterName);
        if(importer == null)
        {
            throw new NotFoundException($"No existe el importador {request.ImporterName}");
        }

        var importedProducts = importer.Import(request.FileStream);

        return importedProducts
            .Select(importedProductCreatorService.CreateImportedProduct)
            .ToList();
    }
}

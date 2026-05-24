using BusinessLogicsAbstractions;
using BusinessLogicsAbstractions.Exceptions;
using BusinessLogicsAbstractions.Imports;
using BusinessLogicsAbstractions.Products;
using Contract.Importers;
using Contract.Products;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLogics;

public class ProductImportService(
    IProductImporterProvider importerProvider,
    IImportedProductCreatorService importedProductCreatorService) : IProductImportService
{
    public List<ProductResponseDto> ImportProducts(ProductImportRequestDto request)
    {
        ValidateRequest(request);

        if(string.IsNullOrWhiteSpace(request.ImporterName))
        {
            throw new ValidationException(
                "El nombre del importador es requerido");
        }

        var importer = FindImporter(request.ImporterName);

        var importedProducts = importer.Import(request.FileStream);

        return importedProducts
            .Select(importedProductCreatorService.CreateImportedProduct)
            .ToList();
    }

    private static void ValidateRequest(ProductImportRequestDto request)
    {
        if(request == null)
        {
            throw new ValidationException(
                "La solicitud de importación es requerida");
        }
    }

    private IProductImporter FindImporter(string importerName)
    {
        var importer = importerProvider
            .GetImporters()
            .FirstOrDefault(i => i.Name == importerName);
        if(importer == null)
        {
            throw new NotFoundException($"No existe el importador {importerName}");
        }

        return importer;
    }
}

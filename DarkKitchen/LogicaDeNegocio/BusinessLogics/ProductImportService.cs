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
    public List<ProductResponseDto> ImportProducts(ProductImportRequestDto request, string email)
    {
        ValidateRequest(request);

        var importer = FindImporter(request.ImporterName);

        var importedProducts = importer.Import(request.FileStream);

        return importedProducts
            .Select(importedProduct =>
                importedProductCreatorService.CreateImportedProduct(
                    importedProduct,
                    email))
            .ToList();
    }

    private static void ValidateRequest(ProductImportRequestDto request)
    {
        if(request == null)
        {
            throw new ValidationException(
                "La solicitud de importación es requerida");
        }

        if(string.IsNullOrWhiteSpace(request.ImporterName))
        {
            throw new ValidationException(
                "El nombre del importador es requerido");
        }

        if(request.FileStream == null)
        {
            throw new ValidationException(
                "El archivo de importación es requerido");
        }

        if(string.IsNullOrWhiteSpace(request.FileName))
        {
            throw new ValidationException(
                "El nombre del archivo es requerido");
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

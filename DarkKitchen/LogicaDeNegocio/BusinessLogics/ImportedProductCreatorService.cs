using BusinessLogicsAbstractions;
using BusinessLogicsAbstractions.Imports;
using BusinessLogicsAbstractions.Products;
using Contract.Importers;
using Contract.Products;

namespace BusinessLogics;

public class ImportedProductCreatorService(IProductCreatorService productCreatorService,
    IImportedProductPhotoConverter importedProductPhotoConverter) : IImportedProductCreatorService
{
    public ProductResponseDto CreateImportedProduct(ImportedProductDto importedProduct)
    {
        var photos = importedProductPhotoConverter.Convert(
            importedProduct.PhotoPaths);

        var product = new ProductDTO(
            importedProduct.Code,
            importedProduct.Name,
            importedProduct.Description,
            importedProduct.CommercialLine,
            importedProduct.Category,
            importedProduct.Price,
            photos);

        return productCreatorService.Create(product);
    }
}

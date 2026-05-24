using Contract.Importers;
using Contract.Products;

namespace BusinessLogicsAbstractions;

public interface IImportedProductCreatorService
{
    ProductResponseDto CreateImportedProduct(ImportedProductDto importedProduct, string email);
}

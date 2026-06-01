using Contract;

namespace BusinessLogicsAbstractions;

public interface IImportedProductCreatorService
{
    ProductResponseDto CreateImportedProduct(ImportedProductDto importedProduct);
}

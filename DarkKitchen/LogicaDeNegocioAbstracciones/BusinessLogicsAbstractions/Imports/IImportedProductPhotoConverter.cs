using Contract.Products;

namespace BusinessLogicsAbstractions.Imports;

public interface IImportedProductPhotoConverter
{
    List<PhotoDTO> Convert(List<string> photoPaths);
}

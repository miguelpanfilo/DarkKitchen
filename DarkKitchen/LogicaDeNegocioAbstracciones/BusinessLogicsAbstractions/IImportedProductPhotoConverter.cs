using Contract.Products;

namespace BusinessLogicsAbstractions;

public interface IImportedProductPhotoConverter
{
    List<PhotoDTO> Convert(List<string> photoPaths);
}

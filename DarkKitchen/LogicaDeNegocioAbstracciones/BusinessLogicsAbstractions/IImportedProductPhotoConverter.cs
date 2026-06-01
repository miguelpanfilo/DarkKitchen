using Contract;

namespace BusinessLogicsAbstractions;

public interface IImportedProductPhotoConverter
{
    List<PhotoDTO> Convert(List<string> photoPaths);
}

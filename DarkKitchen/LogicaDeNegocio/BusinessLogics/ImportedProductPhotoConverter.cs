using BusinessLogicsAbstractions.Imports;
using Contract.Products;

namespace BusinessLogics;

public class ImportedProductPhotoConverter : IImportedProductPhotoConverter
{
    public List<PhotoDTO> Convert(List<string> photoPaths)
    {
        var photos = new List<PhotoDTO>();

        foreach(var path in photoPaths)
        {
            var bytes = File.ReadAllBytes(path);

            photos.Add(new PhotoDTO(
                Path.GetFileName(path),
                Path.GetExtension(path),
                bytes.Length,
                System.Convert.ToBase64String(bytes)));
        }

        return photos;
    }
}
